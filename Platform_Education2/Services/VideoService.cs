using Microsoft.EntityFrameworkCore;
using PlatformEduPro.Contracts.Abstraction;
using PlatformEduPro.Contracts.ErrorHandling;
using PlatformEduPro.DTO.Video;
using PlatformEduPro.Models;
using PlatformEduPro.Services.Interfaces;

namespace PlatformEduPro.Services
{
    public class VideoService:IVideoService
    {

        private readonly EduPlatformDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public VideoService(EduPlatformDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public async Task<List<TbVideoes>> GetAllVideos()
        {
            var result = await _context.TbVideoes.ToListAsync();
            return result;
        }

        public async Task<Result> AddVideoAsync(VideoDto videoDto)
        {
            if (videoDto.VideoFile == null || videoDto.VideoFile.Length == 0)
                return Result.Failure(VideoError.VideoAdding);

           
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "videos");
            Directory.CreateDirectory(uploadsFolder); 
         
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + videoDto.VideoFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await videoDto.VideoFile.CopyToAsync(fileStream);
            }

            
            var video = new TbVideoes
            {
                VideoURL = "/videos/" + uniqueFileName,  
                VideoTitle = videoDto.VideoTitle, 
                SectionId = videoDto.SectionId  
            };

            _context.TbVideoes.Add(video);  
            await _context.SaveChangesAsync(); 
            return Result.Success();
        }

      
        public async Task<Result> DeleteVideoAsync(int id)
        {
            var video = await _context.TbVideoes.FindAsync(id);  
            if (video == null)
                return Result.Failure(VideoError.VideoNOtFound);

            
            string filePath = Path.Combine(_environment.WebRootPath, video.VideoURL.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);  
            }

            
            _context.TbVideoes.Remove(video);
            await _context.SaveChangesAsync(); 

            return Result.Success();
        }


        public async Task<Result<TbVideoes>> GetVideosIdAsync(int VideoId)
        {
            var video= await _context.TbVideoes.FindAsync(VideoId);
            if (video == null)
            {
                return Result.Failure<TbVideoes>(VideoError.VideoNOtFound);
            }
            return Result.Success(video);
           
        }

        
    }
}
