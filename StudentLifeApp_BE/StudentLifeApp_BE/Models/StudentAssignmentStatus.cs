using System;
using System.Collections.Generic;

namespace StudentLifeApp_BE.Models;

public partial class StudentAssignmentStatus
{
    public int Id { get; set; }

    public int AssignmentId { get; set; }

    public int StudentId { get; set; }

    public double? Score { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public virtual Assignment Assignment { get; set; } = null!;

    public virtual User Student { get; set; } = null!;
}
