using PlatformEduPro.Contracts.Abstraction;
using PlatformEduPro.DTO;
using PlatformEduPro.DTO.Question;
using PlatformEduPro.DTO.Section;
using PlatformEduPro.Models;

namespace PlatformEduPro.Services.Interfaces
{
    public interface ISctionService
    {

        Task<Result> AddSection(WriteSectionDto dto);
        Task<List<SectionDto>> GetAllSections();
        Task<Result<SectionDto>> GetSectionById(int id);
        Task<Result> UpdateSection(int id, WriteSectionDto dto);
        Task<Result> DeleteSection(int id);

    }
}
