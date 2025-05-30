using PlatformEduPro.Contracts.Abstraction;
using PlatformEduPro.DTO.Video;
using PlatformEduPro.Models;

namespace PlatformEduPro.Services.Interfaces
{
    public interface IVideoService
    {
        Task<Result> AddVideoAsync(VideoDto videoDto);  
        Task<Result> DeleteVideoAsync(int id);  
        Task<Result<TbVideoes>> GetVideosIdAsync(int VideoId);
        Task<List<TbVideoes>> GetAllVideos();
    }
}
