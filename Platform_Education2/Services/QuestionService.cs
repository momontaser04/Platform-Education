using Microsoft.EntityFrameworkCore;
using PlatformEduPro.Contracts.Abstraction;
using PlatformEduPro.Contracts.ErrorHandling;
using PlatformEduPro.DTO;
using PlatformEduPro.DTO.Question;
using PlatformEduPro.Models;
using PlatformEduPro.Services.Interfaces;

namespace PlatformEduPro.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly EduPlatformDbContext _context;
        private readonly ICacheService _cacheService;

        public QuestionService(EduPlatformDbContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<List<QuestionDto>> GetAllQuestions()
        {
            const string cacheKey = "all_questions";
            var cachedQuestions = await _cacheService.GetAsync<List<QuestionDto>>(cacheKey);

            if (cachedQuestions is not null)
                return cachedQuestions;

            var questions = await _context.TbQuestions
                .Include(q => q.choices)
                .Select(q => new QuestionDto
                {
                    Id = q.Id,
                    QuestionText = q.Text,
                    ExamId = q.ExamId,
                    answers = q.choices.Select(c => new AnswerDto
                    {
                        ChoiceText = c.Text,
                        IsCorrect = c.IsCorrect
                    }).ToList()
                })
                .ToListAsync();

            await _cacheService.SetAsync(cacheKey, questions);
            return questions;
        }

        public async Task<Result<QuestionDto>> GetQuestionById(int id)
        {
            string cacheKey = $"question_{id}";
            var cachedQuestion = await _cacheService.GetAsync<QuestionDto>(cacheKey);

            if (cachedQuestion is not null)
                return Result.Success(cachedQuestion);

            var question = await _context.TbQuestions
                .Include(q => q.choices)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null) return Result.Failure<QuestionDto>(QuestionError.QuestionNOtFound);

            var questionDto = new QuestionDto
            {
                Id = id,
                QuestionText = question.Text,
                ExamId = question.ExamId,
                answers = question.choices.Select(c => new AnswerDto
                {
                    ChoiceText = c.Text,
                    IsCorrect = c.IsCorrect
                }).ToList()
            };

            await _cacheService.SetAsync(cacheKey, questionDto);
            return Result.Success(questionDto);
        }

        public async Task<Result> AddQuestion(QuestionWithAnswer dto)
        {
            try
            {
                var question = new TbQuestions
                {
                    Text = dto.QuestionText,
                    ExamId = dto.ExamId,
                    choices = dto.answers.Select(a => new TbChoices
                    {
                        Text = a.ChoiceText,
                        IsCorrect = a.IsCorrect
                    }).ToList()
                };

                await _context.TbQuestions.AddAsync(question);
                await _context.SaveChangesAsync();

                //  امسح الكاش بعد الإضافة
                await _cacheService.RemoveAsync("all_questions");

                return Result.Success();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Result.Failure(QuestionError.QuestionAdding);
            }
        }

        public async Task<Result> UpdateQuestion(int id, QuestionWithAnswer dto)
        {
            var question = await _context.TbQuestions
                .Include(q => q.choices)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null) return Result.Failure(QuestionError.QuestionNOtFound);

            question.Text = dto.QuestionText;
            question.ExamId = dto.ExamId;

            _context.TbChoices.RemoveRange(question.choices);

            question.choices = dto.answers.Select(a => new TbChoices
            {
                Text = a.ChoiceText,
                IsCorrect = a.IsCorrect
            }).ToList();

            _context.TbQuestions.Update(question);
            await _context.SaveChangesAsync();

            //  امسح الكاش المرتبط بالسؤال ده وكل الأسئلة
            await _cacheService.RemoveAsync("all_questions");
            await _cacheService.RemoveAsync($"question_{id}");

            return Result.Success();
        }

        public async Task<Result> DeleteQuestion(int id)
        {
            var question = await _context.TbQuestions
                .Include(q => q.choices)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null) return Result.Failure(QuestionError.QuestionNOtFound);
            _context.TbChoices.RemoveRange(question.choices);
            _context.TbQuestions.Remove(question);
            await _context.SaveChangesAsync();

            //  امسح الكاش
            await _cacheService.RemoveAsync("all_questions");
            await _cacheService.RemoveAsync($"question_{id}");

            return Result.Success();
        }
    }

}
