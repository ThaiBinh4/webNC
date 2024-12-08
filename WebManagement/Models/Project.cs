using System;
using System.Collections.Generic;

namespace APIwebmoi.Models;

public partial class Project
{
    public int id_project { get; set; }

    public string? Mota { get; set; }

    public string? Title { get; set; }

    public string? OwnerIds { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Column> Columns { get; set; } = new List<Column>();

    public virtual NguoiDung? OwnerIdsNavigation { get; set; }
}
