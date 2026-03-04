using System;
using System.Collections.Generic;

namespace StudentLifeApp_BE.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? StudentCode { get; set; }

    public int RoleId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ICollection<News> News { get; set; } = new List<News>();

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<StudentAssignmentStatus> StudentAssignmentStatuses { get; set; } = new List<StudentAssignmentStatus>();
}
