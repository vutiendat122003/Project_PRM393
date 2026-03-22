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
                    userId = u.UserId,
                    fullName = u.FullName,
                    email = u.Email,
                    studentCode = u.StudentCode
                })
                .FirstOrDefaultAsync();

            if (student == null)
            {
                return NotFound(new { message = "Student not found" });
            }

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
                    if (totalWeight > 0)
                    {
                        averageScore = gradedAssignments.Sum(x =>
                            (x.status.Score ?? 0) / x.assignment.MaxScore * x.assignment.Weight
                        ) / totalWeight * 10;
                    }
                    else
                    {
                        averageScore = gradedAssignments.Average(x =>
                            (x.status.Score ?? 0) / x.assignment.MaxScore * 10
                        );
                    }
                }

                return new
                {
                    subjectId = subject.SubjectId,
                    subjectName = subject.SubjectName,
                    credits = subject.Credits,
                    totalAssignments = totalAssignments,
                    completedAssignments = completedAssignments,
                    completionPercentage = Math.Round(completionPercentage, 1),
                    averageScore = Math.Round(averageScore, 2),
                    isPassing = averageScore >= 5.0
                };
            }).ToList();

            var totalAssignmentsCount = assignments.Count;
            var completedAssignmentsCount = statuses.Count(s => s.Score != null);
            var overallCompletionPercentage = totalAssignmentsCount > 0
                ? (double)completedAssignmentsCount / totalAssignmentsCount * 100
                : 0;

            var overallAverageScore = subjectProgress.Any()
                ? subjectProgress.Average(sp => sp.averageScore)
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
                        assignmentId = assignment.AssignmentId,
                        assignmentTitle = assignment.Title,
                        subjectName = assignment.Subject.SubjectName,
                        score = status.Score,
                        maxScore = assignment.MaxScore,
                        weight = assignment.Weight,
                        submittedAt = status.SubmittedAt,
                        scorePercentage = Math.Round((status.Score ?? 0) / assignment.MaxScore * 10, 2)
                    })
                .ToList();

            var upcomingDeadlines = assignments
                .Where(a => a.Deadline >= DateTime.Now)
                .OrderBy(a => a.Deadline)
                .Take(5)
                .Select(a => new
                {
                    assignmentId = a.AssignmentId,
                    title = a.Title,
                    subjectName = a.Subject.SubjectName,
                    deadline = a.Deadline,
                    maxScore = a.MaxScore,
                    weight = a.Weight,
                    isSubmitted = statuses.Any(s => s.AssignmentId == a.AssignmentId),
                    isGraded = statuses.Any(s => s.AssignmentId == a.AssignmentId && s.Score != null)
                })
                .ToList();

            var dashboard = new
            {
                student = student,
                overallStatistics = new
                {
                    totalSubjects = enrollments.Count,
                    totalAssignments = totalAssignmentsCount,
                    completedAssignments = completedAssignmentsCount,
                    completionPercentage = Math.Round(overallCompletionPercentage, 1),
                    averageScore = Math.Round(overallAverageScore, 2)
                },
                subjectProgress = subjectProgress,
                recentGrades = recentGrades,
                upcomingDeadlines = upcomingDeadlines
            };

            return Ok(dashboard);
        }

        // GET: api/dashboard/teacher/5
        [HttpGet("teacher/{userId}")]
        public async Task<ActionResult<object>> GetTeacherDashboard(int userId)
        {
            var teacher = await _context.Users
                .Where(u => u.UserId == userId && u.RoleId == 2)
                .Select(u => new
                {
                    userId = u.UserId,
                    fullName = u.FullName,
                    email = u.Email
                })
                .FirstOrDefaultAsync();

            if (teacher == null)
            {
                return NotFound(new { message = "Teacher not found" });
            }

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
                    subjectId = subject.SubjectId,
                    subjectName = subject.SubjectName,
                    credits = subject.Credits,
                    enrolledStudents = studentCount,
                    totalAssignments = subjectAssignments.Count,
                    upcomingDeadlines = subjectAssignments.Count(a => a.Deadline >= DateTime.Now)
                };
            }).ToList();

            var recentAssignments = assignments
                .OrderByDescending(a => a.AssignmentId)
                .Take(5)
                .Select(a => new
                {
                    assignmentId = a.AssignmentId,
                    title = a.Title,
                    subjectName = a.Subject.SubjectName,
                    deadline = a.Deadline,
                    maxScore = a.MaxScore,
                    weight = a.Weight,
                    totalSubmissions = allStatuses.Count(s => s.AssignmentId == a.AssignmentId),
                    gradedSubmissions = allStatuses.Count(s => s.AssignmentId == a.AssignmentId && s.Score != null)
                })
                .ToList();

            var pendingSubmissions = allStatuses
                .Where(s => s.Score == null)
                .OrderBy(s => s.SubmittedAt)
                .Join(assignments,
                    status => status.AssignmentId,
                    assignment => assignment.AssignmentId,
                    (status, assignment) => new
                    {
                        statusId = status.Id,
                        assignmentId = assignment.AssignmentId,
                        assignmentTitle = assignment.Title,
                        subjectName = assignment.Subject.SubjectName,
                        studentId = status.StudentId,
                        submittedAt = status.SubmittedAt
                    })
                .Take(10)
                .ToList();

            var dashboard = new
            {
                teacher = teacher,
                overallStatistics = new
                {
                    totalSubjects = teachingSubjects.Count,
                    totalStudents = totalStudents,
                    totalAssignments = assignments.Count,
                    totalSubmissions = totalSubmissions,
                    gradedSubmissions = gradedSubmissions,
                    pendingGrading = pendingGrading
                },
                subjectOverview = subjectOverview,
                recentAssignments = recentAssignments,
                pendingSubmissions = pendingSubmissions
            };

            return Ok(dashboard);
        }

        // GET: api/dashboard/performance-trend/{userId}
        [HttpGet("performance-trend/{userId}")]
        public async Task<ActionResult<object>> GetPerformanceTrend(int userId)
        {
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);

            var grades = await _context.StudentAssignmentStatuses
                .Where(s => s.StudentId == userId &&
                           s.Score != null &&
                           s.SubmittedAt >= sixMonthsAgo)
                .Include(s => s.Assignment)
                .OrderBy(s => s.SubmittedAt)
                .ToListAsync();

            var monthlyPerformance = grades
                .GroupBy(s => new
                {
                    Year = s.SubmittedAt!.Value.Year,
                    Month = s.SubmittedAt.Value.Month
                })
                .Select(g => new
                {
                    year = g.Key.Year,
                    month = g.Key.Month,
                    monthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                    averageScore = Math.Round(
                        g.Average(s => (s.Score ?? 0) / s.Assignment.MaxScore * 10), 2
                    ),
                    totalAssignments = g.Count()
                })
                .OrderBy(m => m.year)
                .ThenBy(m => m.month)
                .ToList();

            return Ok(monthlyPerformance);
        }
    }
}