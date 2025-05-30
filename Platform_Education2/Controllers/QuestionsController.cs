using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlatformEduPro.Contracts.Abstraction;
using PlatformEduPro.Contracts.Const;
using PlatformEduPro.Contracts.Filters;
using PlatformEduPro.DTO;
using PlatformEduPro.DTO.Question;
using PlatformEduPro.DTO.Section;
using PlatformEduPro.DTO.UserAnswer;
using PlatformEduPro.Services;
using PlatformEduPro.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PlatformEduPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {

        IQuestionService questionRepo;
      
        public QuestionsController(IQuestionService questionRepo)
        {
            
            this.questionRepo = questionRepo;
            
        }


        [HttpGet("Get_All_Question")]
        [HasPermission(Permissions.Question_GetAll)]

        public async Task<IActionResult> GetQuestions()
        {
            var result = await questionRepo.GetAllQuestions();
            if (result == null) return BadRequest();
            return Ok(result);
        }


        [HttpGet("Get_Question_by{id}")]
        [HasPermission(Permissions.Question_GetById)]
      
        public async Task<IActionResult> GetById(int id)
        {
            var course = await questionRepo.GetQuestionById(id);
            return course.IsSuccess ? Ok(course.Value) : NotFound(course.Error);
        }

        [HttpPost("AddQuestion")]
        [HasPermission(Permissions.Question_Create)]
      
        public async Task<IActionResult> AddQuestion([FromBody] QuestionWithAnswer dto)
        {
            var result = await questionRepo.AddQuestion(dto);
            return result.IsSuccess ? Ok(result.IsSuccess) : BadRequest(result.Error);
        }






        [HttpPut("UpdateBy{id}")]
        [HasPermission(Permissions.Question_Update)]
        public async Task<IActionResult> UpdateQuestion(int id, [FromBody] QuestionWithAnswer dto)
        {
            var result = await questionRepo.UpdateQuestion(id, dto);
            return result.IsSuccess ? Ok(result.IsSuccess) : BadRequest(result.Error);
        }

        [HttpDelete("DeleteBy{id}")]
        [HasPermission(Permissions.Question_Delete)]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var result = await questionRepo.DeleteQuestion(id);
            return result.IsSuccess ? Ok(result.IsSuccess) : BadRequest(result.Error);
        }




    


    }
}
