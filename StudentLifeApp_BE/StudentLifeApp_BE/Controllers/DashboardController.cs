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
        // Get complete student dashboard data
        [HttpGet("student/{userId}")]
        public async Task<ActionResult<object>> GetStudentDashboard(int userId)
        {
            // Get student info
            var student = await _context.Users
                .Where(u => u.UserID == userId && u.RoleID == 3) // RoleID 3 = Student
                .Select(u => new
                {
                    u.UserID,
                    u.FullName,
                    u.Email,
                    u.StudentCode
                })
                .FirstOrDefaultAsync();

            if (student == null)
            {
                return NotFound(new { message = "Student not found" });
            }

            // Get enrolled subjects
            var enrollments = await _context.Enrollments
                .Where(e => e.UserID == userId && !e.IsTeacher)
                .Include(e => e.Subject)
                .Select(e => e.Subject)
                .ToListAsync();

            var subjectIds = enrollments.Select(s => s.SubjectID).ToList();

            // Get all assignments for enrolled subjects
            var assignments = await _context.Assignments
                .Where(a => subjectIds.Contains(a.SubjectID))
                .Include(a => a.Subject)
                .ToListAsync();

            // Get student's assignment statuses
            var statuses = await _context.StudentAssignmentStatuses
                .Where(s => s.StudentID == userId)
                .ToListAsync();

            // Calculate progress per subject
            var subjectProgress = enrollments.Select(subject =>
            {
                var subjectAssignments = assignments.Where(a => a.SubjectID == subject.SubjectID).ToList();
                var subjectStatuses = statuses.Where(s =>
                    subjectAssignments.Any(a => a.AssignmentID == s.AssignmentID)
                ).ToList();

                var totalAssignments = subjectAssignments.Count;
                var completedAssignments = subjectStatuses.Count(s => s.Score != null);
                var completionPercentage = totalAssignments > 0
                    ? (double)completedAssignments / totalAssignments * 100
                    : 0;

                // Calculate weighted average score
                var gradedAssignments = subjectStatuses
                    .Where(s => s.Score != null)
                    .Join(subjectAssignments,
                        status => status.AssignmentID,
                        assignment => assignment.AssignmentID,
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
                        ) / totalWeight * 10; // Scale to 10
                    }
                    else
                    {
                        // Simple average if no weights
                        averageScore = gradedAssignments.Average(x =>
                            (x.status.Score ?? 0) / x.assignment.MaxScore * 10
                        );
                    }
                }

                return new
                {
                    SubjectID = subject.SubjectID,
                    SubjectName = subject.SubjectName,
                    SubjectCode = subject.SubjectCode,
                    Credits = subject.Credits,
                    TotalAssignments = totalAssignments,
                    CompletedAssignments = completedAssignments,
                    CompletionPercentage = Math.Round(completionPercentage, 1),
                    AverageScore = Math.Round(averageScore, 2),
                    IsPassing = averageScore >= 5.0
                };
            }).ToList();

            // Overall statistics
            var totalAssignmentsCount = assignments.Count;
            var completedAssignmentsCount = statuses.Count(s => s.Score != null);
            var overallCompletionPercentage = totalAssignmentsCount > 0
                ? (double)completedAssignmentsCount / totalAssignmentsCount * 100
                : 0;

            var overallAverageScore = subjectProgress.Any()
                ? subjectProgress.Average(sp => sp.AverageScore)
                : 0;

            // Get recent grades (last 10)
            var recentGrades = statuses
                .Where(s => s.Score != null)
                .OrderByDescending(s => s.SubmittedAt)
                .Take(10)
                .Join(assignments,
                    status => status.AssignmentID,
                    assignment => assignment.AssignmentID,
                    (status, assignment) => new
                    {
                        AssignmentID = assignment.AssignmentID,
                        AssignmentTitle = assignment.Title,
                        SubjectName = assignment.Subject.SubjectName,
                        Score = status.Score,
                        MaxScore = assignment.MaxScore,
                        Weight = assignment.Weight,
                        SubmittedAt = status.SubmittedAt,
                        ScorePercentage = Math.Round((status.Score ?? 0) / assignment.MaxScore * 10, 2)
                    })
                .ToList();

            // Get upcoming deadlines
            var upcomingDeadlines = assignments
                .Where(a => a.Deadline >= DateTime.Now)
                .OrderBy(a => a.Deadline)
                .Take(5)
                .Select(a => new
                {
                    AssignmentID = a.AssignmentID,
                    Title = a.Title,
                    SubjectName = a.Subject.SubjectName,
                    Deadline = a.Deadline,
                    MaxScore = a.MaxScore,
                    Weight = a.Weight,
                    IsSubmitted = statuses.Any(s => s.AssignmentID == a.AssignmentID),
                    IsGraded = statuses.Any(s => s.AssignmentID == a.AssignmentID && s.Score != null)
                })
                .ToList();

            var dashboard = new
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
            };

            return Ok(dashboard);
        }

        // GET: api/dashboard/teacher/5
        // Get complete teacher dashboard data
        [HttpGet("teacher/{userId}")]
        public async Task<ActionResult<object>> GetTeacherDashboard(int userId)
        {
            // Get teacher info
            var teacher = await _context.Users
                .Where(u => u.UserID == userId && u.RoleID == 2) // RoleID 2 = Teacher
                .Select(u => new
                {
                    u.UserID,
                    u.FullName,
                    u.Email
                })
                .FirstOrDefaultAsync();

            if (teacher == null)
            {
                return NotFound(new { message = "Teacher not found" });
            }

            // Get subjects teacher is teaching
            var teachingSubjects = await _context.Enrollments
                .Where(e => e.UserID == userId && e.IsTeacher)
                .Include(e => e.Subject)
                .Select(e => e.Subject)
                .ToListAsync();

            var subjectIds = teachingSubjects.Select(s => s.SubjectID).ToList();

            // Get total students enrolled in teacher's subjects
            var totalStudents = await _context.Enrollments
                .Where(e => subjectIds.Contains(e.SubjectID) && !e.IsTeacher)
                .Select(e => e.UserID)
                .Distinct()
                .CountAsync();

            // Get assignments created by this teacher
            var assignments = await _context.Assignments
                .Where(a => a.CreatedBy == userId)
                .Include(a => a.Subject)
                .ToListAsync();

            // Get submission statistics
            var assignmentIds = assignments.Select(a => a.AssignmentID).ToList();
            var allStatuses = await _context.StudentAssignmentStatuses
                .Where(s => assignmentIds.Contains(s.AssignmentID))
                .ToListAsync();

            var totalSubmissions = allStatuses.Count;
            var gradedSubmissions = allStatuses.Count(s => s.Score != null);
            var pendingGrading = totalSubmissions - gradedSubmissions;

            // Subject overview
            var subjectOverview = teachingSubjects.Select(subject =>
            {
                var subjectAssignments = assignments.Where(a => a.SubjectID == subject.SubjectID).ToList();
                var studentCount = _context.Enrollments
                    .Count(e => e.SubjectID == subject.SubjectID && !e.IsTeacher);

                return new
                {
                    SubjectID = subject.SubjectID,
                    SubjectName = subject.SubjectName,
                    SubjectCode = subject.SubjectCode,
                    Credits = subject.Credits,
                    EnrolledStudents = studentCount,
                    TotalAssignments = subjectAssignments.Count,
                    UpcomingDeadlines = subjectAssignments.Count(a => a.Deadline >= DateTime.Now)
                };
            }).ToList();

            // Recent assignments
            var recentAssignments = assignments
                .OrderByDescending(a => a.AssignmentID)
                .Take(5)
                .Select(a => new
                {
                    AssignmentID = a.AssignmentID,
                    Title = a.Title,
                    SubjectName = a.Subject.SubjectName,
                    Deadline = a.Deadline,
                    MaxScore = a.MaxScore,
                    Weight = a.Weight,
                    TotalSubmissions = allStatuses.Count(s => s.AssignmentID == a.AssignmentID),
                    GradedSubmissions = allStatuses.Count(s => s.AssignmentID == a.AssignmentID && s.Score != null)
                })
                .ToList();

            // Pending submissions to grade
            var pendingSubmissions = allStatuses
                .Where(s => s.Score == null)
                .OrderBy(s => s.SubmittedAt)
                .Join(assignments,
                    status => status.AssignmentID,
                    assignment => assignment.AssignmentID,
                    (status, assignment) => new
                    {
                        StatusID = status.Id,
                        AssignmentID = assignment.AssignmentID,
                        AssignmentTitle = assignment.Title,
                        SubjectName = assignment.Subject.SubjectName,
                        StudentID = status.StudentID,
                        SubmittedAt = status.SubmittedAt
                    })
                .Take(10)
                .ToList();

            var dashboard = new
            {
                Teacher = teacher,
                OverallStatistics = new
                {
                    TotalSubjects = teachingSubjects.Count,
                    TotalStudents = totalStudents,
                    TotalAssignments = assignments.Count,
                    TotalSubmissions = totalSubmissions,
                    GradedSubmissions = gradedSubmissions,
                    PendingGrading = pendingGrading
                },
                SubjectOverview = subjectOverview,
                RecentAssignments = recentAssignments,
                PendingSubmissions = pendingSubmissions
            };

            return Ok(dashboard);
        }

        // GET: api/dashboard/performance-trend/{userId}
        // Get student performance trend (last 6 months)
        [HttpGet("performance-trend/{userId}")]
        public async Task<ActionResult<object>> GetPerformanceTrend(int userId)
        {
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);

            var grades = await _context.StudentAssignmentStatuses
                .Where(s => s.StudentID == userId &&
                           s.Score != null &&
                           s.SubmittedAt >= sixMonthsAgo)
                .Include(s => s.Assignment)
                .OrderBy(s => s.SubmittedAt)
                .ToListAsync();

            // Group by month
            var monthlyPerformance = grades
                .GroupBy(s => new
                {
                    Year = s.SubmittedAt!.Value.Year,
                    Month = s.SubmittedAt.Value.Month
                })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                    AverageScore = Math.Round(
                        g.Average(s => (s.Score ?? 0) / s.Assignment.MaxScore * 10), 2
                    ),
                    TotalAssignments = g.Count()
                })
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
                .ToList();

            return Ok(monthlyPerformance);
        }
    }
}