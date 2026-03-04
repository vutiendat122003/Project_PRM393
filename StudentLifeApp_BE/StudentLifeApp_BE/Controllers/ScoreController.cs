using Microsoft.AspNetCore.Mvc;
using StudentLifeApp_BE.DTOs;
using StudentLifeApp_BE.Services;

namespace StudentLifeApp_BE.Controllers
{
    [ApiController]
    [Route("api/score")]
    public class ScoreController : ControllerBase
    {
        private readonly ScoreService _service;

        public ScoreController(ScoreService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> InsertScore(InsertScoreDTO dto)
        {
            await _service.InsertScore(dto);
            return Ok("Insert success");
        }

        [HttpGet("{studentId}")]
        public async Task<IActionResult> GetScores(int studentId)
        {
            var result = await _service.GetScores(studentId);
            return Ok(result);
        }

        [HttpGet("gpa/{studentId}")]
        public async Task<IActionResult> GetGPA(int studentId)
        {
            var gpa = await _service.GetGPA(studentId);
            return Ok(gpa);
        }
    }
}
