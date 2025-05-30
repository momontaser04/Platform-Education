using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformEduPro.Contracts.Abstraction;
using PlatformEduPro.Contracts.Const;
using PlatformEduPro.Contracts.Filters;
using PlatformEduPro.DTO.Video;
using PlatformEduPro.Services.Interfaces;

namespace PlatformEduPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoesController : ControllerBase
    {
        IVideoService _videoRepository;
        public VideoesController(IVideoService videoRepo)
        {
            _videoRepository = videoRepo;
        }

        [HttpGet("GetAllVideoes")]
        [HasPermission(Permissions.Video_GetAll)]

        public async Task<IActionResult> GetAll()
        {
            var result =await _videoRepository.GetAllVideos();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet("Get_VideoBy{Id}")]
        [HasPermission(Permissions.Video_GetById)]
        public async Task<IActionResult> GetVideosBySection(int Id)
        {
            var videos = await _videoRepository.GetVideosIdAsync(Id);
            return videos.IsSuccess ? Ok(videos.Value) : NotFound(videos.Error);
        }

        [HttpPost("upload_Video")]
        [HasPermission(Permissions.Video_Create)]
    
        public async Task<IActionResult> UploadVideo([FromForm] VideoDto videoDto)
        {
            var result = await _videoRepository.AddVideoAsync(videoDto);
            return result.IsSuccess ? Ok(result.IsSuccess) : BadRequest(result.Error);
        }

        [HttpDelete("Delete_Video{id}")]
        [HasPermission(Permissions.Video_Delete)]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            var result = await _videoRepository.DeleteVideoAsync(id);
            return result.IsSuccess ? Ok(result.IsSuccess) : BadRequest(result.Error);
        }

        
   
    }
}
