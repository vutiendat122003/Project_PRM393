using Microsoft.EntityFrameworkCore;
using StudentLifeApp_BE.DTOs;
using StudentLifeApp_BE.Models;

namespace StudentLifeApp_BE.Services
{
    public class ScoreService
    {
        private readonly LmsDbContext _context;

        public ScoreService(LmsDbContext context)
        {
            _context = context;
        }

        public async Task InsertScore(InsertScoreDTO dto)
        {
            var entity = new StudentAssignmentStatus
            {
                StudentId = dto.StudentID,
                AssignmentId = dto.AssignmentID,
                Score = dto.Score,
                SubmittedAt = DateTime.Now
            };

            _context.StudentAssignmentStatuses.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<object>> GetScores(int studentId)
        {
            var result = await _context.StudentAssignmentStatuses
                .Where(x => x.StudentId == studentId)
                .Select(x => new
                {
                    Subject = x.Assignment.Subject.SubjectName,
                    Credits = x.Assignment.Subject.Credits,
                    Score = x.Score
                })
                .ToListAsync();

            return result.Cast<object>().ToList();
        }

        public async Task<double> GetGPA(int studentId)
        {
            var data = await _context.StudentAssignmentStatuses
                .Where(x => x.StudentId == studentId)
                .Select(x => new
                {
                    x.Score,
                    Credits = x.Assignment.Subject.Credits
                })
                .ToListAsync();

            double total = 0;
            int totalCredits = 0;

            foreach (var item in data)
            {
                if (item.Score.HasValue)
                {
                    total += item.Score.Value * item.Credits;
                    totalCredits += item.Credits;
                }
            }

            if (totalCredits == 0)
                return 0;

            return total / totalCredits;
        }
    }
}
