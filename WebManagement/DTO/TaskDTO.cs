
namespace WebManagement.DTO
{
    public class TaskDTO
    {
        public int id_task { get; set; }

        public int? id_column { get; set; }

        public string? Title { get; set; }

        public string? Ketqua { get; set; }

        public string? Mota { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdateAt { get; set; }

        public DateTime? Deadline { get; set; }
    }
}
