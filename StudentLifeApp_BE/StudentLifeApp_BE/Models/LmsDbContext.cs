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
        => optionsBuilder.UseSqlServer("server =DESKTOP-22Q8721\\SQLEXPRESS; database = LMS_DB; uid=sa;pwd=12345678;Trusted_Connection=True;Encrypt=False");

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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
