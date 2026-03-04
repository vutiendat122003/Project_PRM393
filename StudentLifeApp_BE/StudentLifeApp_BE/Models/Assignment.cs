using System;
using System.Collections.Generic;

namespace StudentLifeApp_BE.Models;

public partial class Assignment
{
    public int AssignmentId { get; set; }

    public int SubjectId { get; set; }

    public string Title { get; set; } = null!;

    public DateTime Deadline { get; set; }

    public double MaxScore { get; set; }

    public double Weight { get; set; }

    public int CreatedBy { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<StudentAssignmentStatus> StudentAssignmentStatuses { get; set; } = new List<StudentAssignmentStatus>();

    public virtual Subject Subject { get; set; } = null!;
}
