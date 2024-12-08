using System;
using System.Collections.Generic;

namespace APIwebmoi.Models;

public partial class NguoiDung
{
    public string IdUser { get; set; } = null!;

    public string? Email { get; set; }

    public string? Pass { get; set; }

    public string? Username { get; set; }

    public string? Avatar { get; set; }

    public int? IdRole { get; set; }

    public bool? IsActive { get; set; }

    public string? Fullname { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual VaiTro? IdRoleNavigation { get; set; }

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<UserTask> UserTasks { get; set; } = new List<UserTask>();
}
