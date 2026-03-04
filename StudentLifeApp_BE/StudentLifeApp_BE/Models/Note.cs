using System;
using System.Collections.Generic;

namespace StudentLifeApp_BE.Models;

public partial class Note
{
    public int NoteId { get; set; }

    public int UserId { get; set; }

    public string? Title { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsPinned { get; set; }

    public virtual User User { get; set; } = null!;
}
