using PlatformEduPro.Contracts.Abstraction;
using PlatformEduPro.DTO;
using PlatformEduPro.DTO.Command;
using PlatformEduPro.DTO.Question;
using PlatformEduPro.Models;

namespace PlatformEduPro.Services.Interfaces
{
    public interface IQuestionService
    {
        Task<List<QuestionDto>> GetAllQuestions();
        Task<Result<QuestionDto>> GetQuestionById(int id);
        Task<Result> AddQuestion(QuestionWithAnswer dto);
        Task<Result> UpdateQuestion(int id, QuestionWithAnswer dto);
        Task<Result> DeleteQuestion(int id);


    }
}
