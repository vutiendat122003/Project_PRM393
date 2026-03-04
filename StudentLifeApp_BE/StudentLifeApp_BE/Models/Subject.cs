using System;
using System.Collections.Generic;

namespace StudentLifeApp_BE.Models;

public partial class Subject
{
    public int SubjectId { get; set; }

    public string SubjectName { get; set; } = null!;

    public int Credits { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
