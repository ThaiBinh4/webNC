using System;
using System.Collections.Generic;

namespace APIwebmoi.Models;

public partial class Column
{
    public int id_column { get; set; }

    public int? IdProject { get; set; }

    public string? Title { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Project? IdProjectNavigation { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
