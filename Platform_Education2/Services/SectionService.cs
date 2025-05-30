using Microsoft.EntityFrameworkCore;
using PlatformEduPro.Contracts.Abstraction;
using PlatformEduPro.Contracts.ErrorHandling;
using PlatformEduPro.DTO.Section;
using PlatformEduPro.DTO.Video;
using PlatformEduPro.Models;
using PlatformEduPro.Services.Interfaces;

namespace PlatformEduPro.Services
{
    public class SectionService : ISctionService
    {
        EduPlatformDbContext _context;
        public SectionService(EduPlatformDbContext context)
        {
           _context = context;
        }


        public async Task<Result> AddSection(WriteSectionDto dto)
        {
            try
            {
                var section = new TbSections
                {
                    SectionTitle = dto.SectionName,
                    CourseId = dto.CourseId,
                    exam = !string.IsNullOrEmpty(dto.ExamTitle) ? new TbExam { Title = dto.ExamTitle } : null
                };

                _context.TbSections.Add(section);
                await _context.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Result.Failure(SectionError.SectionAdding);
            }
        }



        public async Task<List<SectionDto>> GetAllSections()
        {
            return await _context.TbSections
                .Include(s => s.videoes) 
                .Include(s => s.exam)   
                .Select(s => new SectionDto
                {
                    Id = s.Id,
                    SectionName = s.SectionTitle,
                    CourseId = s.CourseId,
                    ExamTitle = s.exam != null ? s.exam.Title : null,
                    ExamId = s.exam != null ? s.exam.Id : 0,
                    videoTitle = s.videoes.Select(v => v.VideoTitle).ToList()
                })
                .ToListAsync();
        }


        public async Task<Result<SectionDto>> GetSectionById(int id)
        {
            var section = await _context.TbSections
                .Include(s => s.videoes) 
                .Include(s => s.exam)   
                .FirstOrDefaultAsync(s => s.Id == id);

            if (section == null)
                return Result.Failure<SectionDto>(SectionError.SectionNOtFound);

            var SectionId= new SectionDto
            {
                Id = section.Id,
                SectionName = section.SectionTitle,
                CourseId = section.CourseId,
                ExamId = section.exam != null ? section.exam.Id : 0,
                ExamTitle = section.exam != null ? section.exam.Title : null,
                videoTitle = section.videoes.Select(v => v.VideoTitle).ToList()
            };
            return Result.Success(SectionId);
        }



        public async Task<Result> UpdateSection(int id, WriteSectionDto dto)
        {
            var section = await _context.TbSections
                .Include(s => s.exam)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (section == null) return Result.Failure(SectionError.SectionNOtFound);

            section.SectionTitle = dto.SectionName;

           
            if (!string.IsNullOrEmpty(dto.ExamTitle))
            {
                if (section.exam == null)
                {
                    section.exam = new TbExam { Title = dto.ExamTitle };
                    _context.tbExams.Add(section.exam);
                }
                else
                {
                    section.exam.Title = dto.ExamTitle;
                }
            }

            await _context.SaveChangesAsync();
            return Result.Success();
        }




        public async Task<Result> DeleteSection(int id)
        {
            var section = await _context.TbSections
                .Include(s => s.videoes)
                .Include(s => s.exam)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (section == null) return Result.Failure(SectionError.SectionNOtFound);

         
            if (section.videoes != null && section.videoes.Any())
            {
                _context.TbVideoes.RemoveRange(section.videoes);
            }

   
            if (section.exam != null)
            {
                _context.tbExams.Remove(section.exam);
            }

    
            _context.TbSections.Remove(section);

            await _context.SaveChangesAsync();
            return Result.Success();
        }


    }
}
