using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace StudentLifeApp_BE.Models;

public partial class LmsDbContext : DbContext
{
    public LmsDbContext()
    {
    }

    public LmsDbContext(DbContextOptions<LmsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<Note> Notes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<StudentAssignmentStatus> StudentAssignmentStatuses { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server = localhost; Database=LMS_DB;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__Assignme__32499E57FC1C1C0A");

            entity.Property(e => e.AssignmentId).HasColumnName("AssignmentID");
            entity.Property(e => e.Deadline).HasColumnType("datetime");
            entity.Property(e => e.MaxScore).HasDefaultValue(10.0);
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Assignment_Teacher");

            entity.HasOne(d => d.Subject).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Assignment_Subject");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PK__Enrollme__7F6877FB6B53A844");

            entity.HasIndex(e => new { e.UserId, e.SubjectId }, "UQ_Enrollment").IsUnique();

            entity.Property(e => e.EnrollmentId).HasColumnName("EnrollmentID");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Subject).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enroll_Subject");

            entity.HasOne(d => d.User).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enroll_User");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.NewsId).HasName("PK__News__954EBDD396EF45C1");

            entity.Property(e => e.NewsId).HasColumnName("NewsID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.TargetRoleId).HasColumnName("TargetRoleID");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.News)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_News_User");

            entity.HasOne(d => d.TargetRole).WithMany(p => p.News)
                .HasForeignKey(d => d.TargetRoleId)
                .HasConstraintName("FK_News_Role");
        });

        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.NoteId).HasName("PK__Notes__EACE357F98EE0990");

            entity.Property(e => e.NoteId).HasColumnName("NoteID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsPinned).HasDefaultValue(false);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Notes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Notes_User");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3ABF28B166");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160D9F003C9").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<StudentAssignmentStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StudentA__3214EC2749C02C47");

            entity.ToTable("StudentAssignmentStatus");

            entity.HasIndex(e => new { e.AssignmentId, e.StudentId }, "UQ_Student_Assignment").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AssignmentId).HasColumnName("AssignmentID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.SubmittedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Assignment).WithMany(p => p.StudentAssignmentStatuses)
                .HasForeignKey(d => d.AssignmentId)
                .HasConstraintName("FK_Status_Assignment");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentAssignmentStatuses)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Status_Student");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__Subjects__AC1BA3880C5F783A");

            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.SubjectName).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC75CB51E4");

            entity.HasIndex(e => e.StudentCode, "UQ__Users__1FC886045B65753A").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105344FFD76AF").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.StudentCode).HasMaxLength(20);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        // ---------- Seeding bằng Fluent API (HasData) ----------
        // Lưu ý: khi dùng HasData phải chỉ định các giá trị khóa (ids) tĩnh.

        // Roles (3 records)
        modelBuilder.Entity<Role>().HasData(
            new Role { RoleId = 1, RoleName = "Admin" },
            new Role { RoleId = 2, RoleName = "Teacher" },
            new Role { RoleId = 3, RoleName = "Student" }
        );

        // Users (6 records: 1 admin, 1 teacher, 4 students)
        modelBuilder.Entity<User>().HasData(
            new User
            {
                UserId = 1,
                FullName = "Admin User",
                Email = "admin@lms.local",
                PasswordHash = "admin-placeholder-hash",
                RoleId = 1,
                CreatedAt = new DateTime(2026, 3, 4, 8, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                UserId = 2,
                FullName = "Teacher One",
                Email = "teacher1@lms.local",
                PasswordHash = "teacher-placeholder-hash",
                RoleId = 2,
                CreatedAt = new DateTime(2026, 3, 4, 8, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                UserId = 3,
                FullName = "Student One",
                Email = "student1@lms.local",
                PasswordHash = "student-placeholder-hash",
                RoleId = 3,
                StudentCode = "S001",
                CreatedAt = new DateTime(2026, 3, 4, 8, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                UserId = 4,
                FullName = "Student Two",
                Email = "student2@lms.local",
                PasswordHash = "student2-placeholder-hash",
                RoleId = 3,
                StudentCode = "S002",
                CreatedAt = new DateTime(2026, 3, 4, 8, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                UserId = 5,
                FullName = "Student Three",
                Email = "student3@lms.local",
                PasswordHash = "student3-placeholder-hash",
                RoleId = 3,
                StudentCode = "S003",
                CreatedAt = new DateTime(2026, 3, 4, 8, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                UserId = 6,
                FullName = "Student Four",
                Email = "student4@lms.local",
                PasswordHash = "student4-placeholder-hash",
                RoleId = 3,
                StudentCode = "S004",
                CreatedAt = new DateTime(2026, 3, 4, 8, 0, 0, DateTimeKind.Utc)
            }
        );

        // Subjects (4 records)
        modelBuilder.Entity<Subject>().HasData(
            new Subject { SubjectId = 1, SubjectName = "Mathematics", Credits = 3, Description = "Basic mathematics and algebra" },
            new Subject { SubjectId = 2, SubjectName = "Physics", Credits = 4, Description = "Introductory physics and mechanics" },
            new Subject { SubjectId = 3, SubjectName = "English", Credits = 2, Description = "English communication and writing" },
            new Subject { SubjectId = 4, SubjectName = "Chemistry", Credits = 3, Description = "General chemistry and reactions" }
        );

        // Assignments (6 records: từng subject có 1-2 assignments)
        modelBuilder.Entity<Assignment>().HasData(
            new Assignment
            {
                AssignmentId = 1,
                Title = "Math Homework 1",
                Deadline = new DateTime(2026, 3, 11, 23, 59, 0, DateTimeKind.Utc),
                MaxScore = 10,
                Weight = 0.2,
                SubjectId = 1,
                CreatedBy = 2
            },
            new Assignment
            {
                AssignmentId = 2,
                Title = "Math Homework 2",
                Deadline = new DateTime(2026, 3, 18, 23, 59, 0, DateTimeKind.Utc),
                MaxScore = 10,
                Weight = 0.2,
                SubjectId = 1,
                CreatedBy = 2
            },
            new Assignment
            {
                AssignmentId = 3,
                Title = "Physics Lab Report",
                Deadline = new DateTime(2026, 3, 14, 23, 59, 0, DateTimeKind.Utc),
                MaxScore = 15,
                Weight = 0.3,
                SubjectId = 2,
                CreatedBy = 2
            },
            new Assignment
            {
                AssignmentId = 4,
                Title = "Physics Experiment",
                Deadline = new DateTime(2026, 3, 21, 23, 59, 0, DateTimeKind.Utc),
                MaxScore = 15,
                Weight = 0.3,
                SubjectId = 2,
                CreatedBy = 2
            },
            new Assignment
            {
                AssignmentId = 5,
                Title = "English Essay",
                Deadline = new DateTime(2026, 3, 9, 23, 59, 0, DateTimeKind.Utc),
                MaxScore = 20,
                Weight = 0.5,
                SubjectId = 3,
                CreatedBy = 2
            },
            new Assignment
            {
                AssignmentId = 6,
                Title = "Chemistry Lab",
                Deadline = new DateTime(2026, 3, 17, 23, 59, 0, DateTimeKind.Utc),
                MaxScore = 12,
                Weight = 0.25,
                SubjectId = 4,
                CreatedBy = 2
            }
        );

        // Enrollments (8 records: teacher + students enrolled in subjects)
        modelBuilder.Entity<Enrollment>().HasData(
            // Teacher enrollments in all subjects
            new Enrollment { EnrollmentId = 1, UserId = 2, SubjectId = 1, IsTeacher = true },
            new Enrollment { EnrollmentId = 2, UserId = 2, SubjectId = 2, IsTeacher = true },
            new Enrollment { EnrollmentId = 3, UserId = 2, SubjectId = 3, IsTeacher = true },
            new Enrollment { EnrollmentId = 4, UserId = 2, SubjectId = 4, IsTeacher = true },

            // Student enrollments
            new Enrollment { EnrollmentId = 5, UserId = 3, SubjectId = 1, IsTeacher = false },
            new Enrollment { EnrollmentId = 6, UserId = 3, SubjectId = 2, IsTeacher = false },
            new Enrollment { EnrollmentId = 7, UserId = 4, SubjectId = 1, IsTeacher = false },
            new Enrollment { EnrollmentId = 8, UserId = 4, SubjectId = 3, IsTeacher = false }
        );

        // News (3 records)
        modelBuilder.Entity<News>().HasData(
            new News
            {
                NewsId = 1,
                Title = "Welcome to LMS",
                Content = "This is a seeded announcement for all users. Welcome to our Learning Management System!",
                CreatedBy = 1,
                TargetRoleId = null,
                CreatedAt = new DateTime(2026, 3, 4, 8, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new News
            {
                NewsId = 2,
                Title = "System Maintenance",
                Content = "System maintenance will occur on March 5th, 2026 from 2 AM to 4 AM UTC.",
                CreatedBy = 1,
                TargetRoleId = null,
                CreatedAt = new DateTime(2026, 3, 4, 9, 30, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new News
            {
                NewsId = 3,
                Title = "Assignment Deadline Reminder",
                Content = "Please note that all assignments must be submitted before the deadline.",
                CreatedBy = 2,
                TargetRoleId = 3,
                CreatedAt = new DateTime(2026, 3, 4, 10, 0, 0, DateTimeKind.Utc),
                IsActive = true
            }
        );

        // Notes (4 records: students' personal notes)
        modelBuilder.Entity<Note>().HasData(
            new Note
            {
                NoteId = 1,
                UserId = 3,
                Title = "Study Plan",
                Content = "Study at least 1 hour/day for mathematics. Focus on algebra and geometry.",
                CreatedAt = new DateTime(2026, 3, 4, 8, 0, 0, DateTimeKind.Utc),
                IsPinned = true
            },
            new Note
            {
                NoteId = 2,
                UserId = 3,
                Title = "Physics Notes",
                Content = "Review Newton's laws and kinematics before the next class.",
                CreatedAt = new DateTime(2026, 3, 4, 12, 0, 0, DateTimeKind.Utc),
                IsPinned = false
            },
            new Note
            {
                NoteId = 3,
                UserId = 4,
                Title = "English Vocabulary",
                Content = "Learn 10 new vocabulary words every day for the essay assignment.",
                CreatedAt = new DateTime(2026, 3, 4, 14, 0, 0, DateTimeKind.Utc),
                IsPinned = false
            },
            new Note
            {
                NoteId = 4,
                UserId = 5,
                Title = "Chemistry Lab Preparation",
                Content = "Prepare lab equipment and read the experiment procedure carefully.",
                CreatedAt = new DateTime(2026, 3, 4, 15, 30, 0, DateTimeKind.Utc),
                IsPinned = true
            }
        );

        // StudentAssignmentStatus (5 records: student submissions with scores)
        modelBuilder.Entity<StudentAssignmentStatus>().HasData(
            new StudentAssignmentStatus
            {
                Id = 1,
                AssignmentId = 1,
                StudentId = 3,
                Score = 9.0,
                SubmittedAt = new DateTime(2026, 3, 3, 12, 0, 0, DateTimeKind.Utc)
            },
            new StudentAssignmentStatus
            {
                Id = 2,
                AssignmentId = 1,
                StudentId = 4,
                Score = 8.5,
                SubmittedAt = new DateTime(2026, 3, 3, 14, 0, 0, DateTimeKind.Utc)
            },
            new StudentAssignmentStatus
            {
                Id = 3,
                AssignmentId = 3,
                StudentId = 3,
                Score = 13.0,
                SubmittedAt = new DateTime(2026, 3, 2, 10, 0, 0, DateTimeKind.Utc)
            },
            new StudentAssignmentStatus
            {
                Id = 4,
                AssignmentId = 5,
                StudentId = 4,
                Score = 18.0,
                SubmittedAt = new DateTime(2026, 3, 1, 8, 0, 0, DateTimeKind.Utc)
            },
            new StudentAssignmentStatus
            {
                Id = 5,
                AssignmentId = 6,
                StudentId = 5,
                Score = 11.0,
                SubmittedAt = new DateTime(2026, 3, 4, 11, 0, 0, DateTimeKind.Utc)
            }
        );

        // ---------- end seeding ----------

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
