using Microsoft.AspNetCore.Mvc;
using StudentLifeApp_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace StudentLifeApp_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentsController : ControllerBase
    {
        private readonly LmsDbContext _context;

        public AssignmentsController(LmsDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> CreateAssignment([FromBody] Assignment assignment)
        {
            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();

            return Ok(assignment);
        }
        [HttpGet("subject/{subjectId}")]
        public async Task<IActionResult> GetAssignmentsBySubject(int subjectId)
        {
            var assignments = await _context.Assignments
                .Where(a => a.SubjectId == subjectId)
                .ToListAsync();

            return Ok(assignments);
        }
    }
}