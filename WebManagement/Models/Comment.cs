using System;
using System.Collections.Generic;

namespace APIwebmoi.Models;

public partial class Comment
{
    public int IdComment { get; set; }

    public string? IdUser { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual NguoiDung? IdUserNavigation { get; set; }
}
