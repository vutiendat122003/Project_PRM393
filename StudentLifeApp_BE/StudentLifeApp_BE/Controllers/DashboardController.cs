using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentLifeApp_BE.Models;

namespace StudentLifeApp_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly LmsDbContext _context;

        public DashboardController(LmsDbContext context)
        {
            _context = context;
        }

        // GET: api/dashboard/student/5
        [HttpGet("student/{userId}")]
        public async Task<ActionResult<object>> GetStudentDashboard(int userId)
        {
            var student = await _context.Users
                .Where(u => u.UserId == userId && u.RoleId == 3)
                .Select(u => new
                {
                    u.UserId,
                    u.FullName,
                    u.Email,
                    u.StudentCode
                })
                .FirstOrDefaultAsync();

            if (student == null)
                return NotFound(new { message = "Student not found" });

            var enrollments = await _context.Enrollments
                .Where(e => e.UserId == userId && !e.IsTeacher)
                .Include(e => e.Subject)
                .Select(e => e.Subject)
                .ToListAsync();

            var subjectIds = enrollments.Select(s => s.SubjectId).ToList();

            var assignments = await _context.Assignments
                .Where(a => subjectIds.Contains(a.SubjectId))
                .Include(a => a.Subject)
                .ToListAsync();

            var statuses = await _context.StudentAssignmentStatuses
                .Where(s => s.StudentId == userId)
                .ToListAsync();

            var subjectProgress = enrollments.Select(subject =>
            {
                var subjectAssignments = assignments.Where(a => a.SubjectId == subject.SubjectId).ToList();
                var subjectStatuses = statuses.Where(s =>
                    subjectAssignments.Any(a => a.AssignmentId == s.AssignmentId)
                ).ToList();

                var totalAssignments = subjectAssignments.Count;
                var completedAssignments = subjectStatuses.Count(s => s.Score != null);
                var completionPercentage = totalAssignments > 0
                    ? (double)completedAssignments / totalAssignments * 100
                    : 0;

                var gradedAssignments = subjectStatuses
                    .Where(s => s.Score != null)
                    .Join(subjectAssignments,
                        status => status.AssignmentId,
                        assignment => assignment.AssignmentId,
                        (status, assignment) => new { status, assignment })
                    .ToList();

                double averageScore = 0;
                if (gradedAssignments.Any())
                {
                    var totalWeight = gradedAssignments.Sum(x => x.assignment.Weight);
                    averageScore = totalWeight > 0
                        ? gradedAssignments.Sum(x =>
                            (x.status.Score ?? 0) / x.assignment.MaxScore * x.assignment.Weight
                          ) / totalWeight * 10
                        : gradedAssignments.Average(x =>
                            (x.status.Score ?? 0) / x.assignment.MaxScore * 10
                          );
                }

                return new
                {
                    SubjectId = subject.SubjectId,
                    subject.SubjectName,
                    subject.Credits,
                    TotalAssignments = totalAssignments,
                    CompletedAssignments = completedAssignments,
                    CompletionPercentage = Math.Round(completionPercentage, 1),
                    AverageScore = Math.Round(averageScore, 2),
                    IsPassing = averageScore >= 5.0
                };
            }).ToList();

            var totalAssignmentsCount = assignments.Count;
            var completedAssignmentsCount = statuses.Count(s => s.Score != null);
            var overallCompletionPercentage = totalAssignmentsCount > 0
                ? (double)completedAssignmentsCount / totalAssignmentsCount * 100
                : 0;

            var overallAverageScore = subjectProgress.Any()
                ? subjectProgress.Average(sp => sp.AverageScore)
                : 0;

            var recentGrades = statuses
                .Where(s => s.Score != null)
                .OrderByDescending(s => s.SubmittedAt)
                .Take(10)
                .Join(assignments,
                    status => status.AssignmentId,
                    assignment => assignment.AssignmentId,
                    (status, assignment) => new
                    {
                        AssignmentId = assignment.AssignmentId,
                        AssignmentTitle = assignment.Title,
                        SubjectName = assignment.Subject.SubjectName,
                        status.Score,
                        assignment.MaxScore,
                        assignment.Weight,
                        status.SubmittedAt,
                        ScorePercentage = Math.Round((status.Score ?? 0) / assignment.MaxScore * 10, 2)
                    })
                .ToList();

            var upcomingDeadlines = assignments
                .Where(a => a.Deadline >= DateTime.Now)
                .OrderBy(a => a.Deadline)
                .Take(5)
                .Select(a => new
                {
                    AssignmentId = a.AssignmentId,
                    a.Title,
                    SubjectName = a.Subject.SubjectName,
                    a.Deadline,
                    a.MaxScore,
                    a.Weight,
                    IsSubmitted = statuses.Any(s => s.AssignmentId == a.AssignmentId),
                    IsGraded = statuses.Any(s => s.AssignmentId == a.AssignmentId && s.Score != null)
                })
                .ToList();

            return Ok(new
            {
                Student = student,
                OverallStatistics = new
                {
                    TotalSubjects = enrollments.Count,
                    TotalAssignments = totalAssignmentsCount,
                    CompletedAssignments = completedAssignmentsCount,
                    CompletionPercentage = Math.Round(overallCompletionPercentage, 1),
                    AverageScore = Math.Round(overallAverageScore, 2)
                },
                SubjectProgress = subjectProgress,
                RecentGrades = recentGrades,
                UpcomingDeadlines = upcomingDeadlines
            });
        }

        // GET: api/dashboard/teacher/5
        [HttpGet("teacher/{userId}")]
        public async Task<ActionResult<object>> GetTeacherDashboard(int userId)
        {
            var teacher = await _context.Users
                .Where(u => u.UserId == userId && u.RoleId == 2)
                .Select(u => new
                {
                    u.UserId,
                    u.FullName,
                    u.Email
                })
                .FirstOrDefaultAsync();

            if (teacher == null)
                return NotFound(new { message = "Teacher not found" });

            var teachingSubjects = await _context.Enrollments
                .Where(e => e.UserId == userId && e.IsTeacher)
                .Include(e => e.Subject)
                .Select(e => e.Subject)
                .ToListAsync();

            var subjectIds = teachingSubjects.Select(s => s.SubjectId).ToList();

            var totalStudents = await _context.Enrollments
                .Where(e => subjectIds.Contains(e.SubjectId) && !e.IsTeacher)
                .Select(e => e.UserId)
                .Distinct()
                .CountAsync();

            var assignments = await _context.Assignments
                .Where(a => a.CreatedBy == userId)
                .Include(a => a.Subject)
                .ToListAsync();

            var assignmentIds = assignments.Select(a => a.AssignmentId).ToList();
            var allStatuses = await _context.StudentAssignmentStatuses
                .Where(s => assignmentIds.Contains(s.AssignmentId))
                .ToListAsync();

            var totalSubmissions = allStatuses.Count;
            var gradedSubmissions = allStatuses.Count(s => s.Score != null);
            var pendingGrading = totalSubmissions - gradedSubmissions;

            var subjectOverview = teachingSubjects.Select(subject =>
            {
                var subjectAssignments = assignments.Where(a => a.SubjectId == subject.SubjectId).ToList();
                var studentCount = _context.Enrollments
                    .Count(e => e.SubjectId == subject.SubjectId && !e.IsTeacher);

                return new
                {
                    SubjectId = subject.SubjectId,
                    subject.SubjectName,
                    subject.Credits,
                    EnrolledStudents = studentCount,
                    TotalAssignments = subjectAssignments.Count,
                    UpcomingDeadlines = subjectAssignments.Count(a => a.Deadline >= DateTime.Now)
                };
            }).ToList();

            var recentAssignments = assignments
                .OrderByDescending(a => a.AssignmentId)
                .Take(5)
                .Select(a => new
                {
                    AssignmentId = a.AssignmentId,
                    a.Title,
                    SubjectName = a.Subject.SubjectName,
                    a.Deadline,
                    a.MaxScore,
                    a.Weight,
                    TotalSubmissions = allStatuses.Count(s => s.AssignmentId == a.AssignmentId),
                    GradedSubmissions = allStatuses.Count(s => s.AssignmentId == a.AssignmentId && s.Score != null)
                })
                .ToList();

            var pendingSubmissions = allStatuses
                .Where(s => s.Score == null)
                .OrderBy(s => s.SubmittedAt)
                .Join(assignments,
                    status => status.AssignmentId,
                    assignment => assignment.AssignmentId,
                    (status, assignment) => new