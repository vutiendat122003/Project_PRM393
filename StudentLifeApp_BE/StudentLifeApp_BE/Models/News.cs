using System;
using System.Collections.Generic;

namespace StudentLifeApp_BE.Models;

public partial class News
{
    public int NewsId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public int CreatedBy { get; set; }

    public int? TargetRoleId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Role? TargetRole { get; set; }
}
