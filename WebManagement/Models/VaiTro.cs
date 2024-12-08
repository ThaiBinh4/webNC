using System;
using System.Collections.Generic;

namespace APIwebmoi.Models;

public partial class VaiTro
{
    public int IdRole { get; set; }

    public string? RoleName { get; set; }

    public virtual ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();
}
