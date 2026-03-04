using System;
using System.Collections.Generic;

namespace StudentLifeApp_BE.Models;

public partial class Enrollment
{
    public int EnrollmentId { get; set; }

    public int UserId { get; set; }

    public int SubjectId { get; set; }

    public bool IsTeacher { get; set; }

    public virtual Subject Subject { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
