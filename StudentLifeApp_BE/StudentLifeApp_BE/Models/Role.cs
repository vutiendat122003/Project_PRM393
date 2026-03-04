using System;
using System.Collections.Generic;

namespace StudentLifeApp_BE.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<News> News { get; set; } = new List<News>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
