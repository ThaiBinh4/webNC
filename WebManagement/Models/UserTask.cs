using System;
using System.Collections.Generic;

namespace APIwebmoi.Models;

public partial class UserTask
{
    public int IdUserTask { get; set; }

    public string? IdUser { get; set; }

    public int? IdTask { get; set; }

    public virtual Task? IdTaskNavigation { get; set; }

    public virtual NguoiDung? IdUserNavigation { get; set; }
}
