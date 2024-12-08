using APIwebmoi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIwebmoi.Controllers
{
    public class ThongKeController : Controller
    {
            private readonly WebNangCaoQlda2Context _context;
            public IActionResult Index()
            {
                return View();
            }
            public ThongKeController(WebNangCaoQlda2Context context)
            {
                _context = context;
            }
        // thống kê hiển thị danh sách task theo dự án
        [HttpGet("tasks-by-project/{projectId}")]
        public IActionResult GetTasksByProject(int projectId)
        {
            var result = _context.Tasks
                .Where(t => t.IdColumnNavigation.IdProject == projectId) // Lọc theo dự án
                .Select(t => new
                {
                    IdTask = t.id_task,
                    TaskName = t.Title,
                    Members = _context.UserTasks
                        .Where(ut => ut.IdTask == t.id_task)
                        .Select(ut => ut.IdUserNavigation.Fullname)
                        .ToList(), // Lấy danh sách thành viên
                    Deadline = t.Deadline,
                    CompletionStatus = t.Ketqua.Contains("Hoàn Thành")
                        ? "Hoàn thành"
                        : "Chưa hoàn thành" // Dựa trên giá trị của 'ketqua'
                }).ToList();

            return Ok(result);
        }
        [HttpGet("tasks-by-project")]
        public IActionResult GetAllTasksByProject()
        {
            var result = _context.Tasks
                .Join(_context.Columns,
                      task => task.id_column,
                      col => col.id_column,
                      (task, col) => new { task, col })
                .Join(_context.Projects,
                      colTask => colTask.col.IdProject,
                      project => project.id_project,
                      (colTask, project) => new { colTask.task, project })
                .Select(tp => new
                {
                    idProject = tp.project.id_project,
                    ProjectName = tp.project.Title, // Tên dự án
                    TaskName = tp.task.Title, // Tên công việc
                    Members = _context.UserTasks
                        .Where(ut => ut.IdTask == tp.task.id_task)
                        .Select(ut => ut.IdUserNavigation.Fullname)
                        .ToList(), // Danh sách thành viên
                    Deadline = tp.task.Deadline, // Hạn chót
                    CompletionStatus = tp.task.Ketqua.Contains("Hoàn Thành") ? "Hoàn thành" : "Chưa hoàn thành" // Trạng thái hoàn thành
                }).ToList(); // Chuyển thành danh sách

            if (result == null || result.Count == 0)
            {
                return NotFound(new { message = "Không tìm thấy công việc nào." });
            }

            return Ok(result); // Trả về kết quả dưới dạng JSON
        }


        // thống kê dự án 
        [HttpGet("tasks/{projectId}")]
        public IActionResult GetTasksByProject1(int projectId)
        {
            // Lấy danh sách task theo projectId
            var projectTasks = _context.Tasks
                .Where(t => t.IdColumnNavigation.IdProject == projectId)
                .Select(t => new
                {
                    t.id_task,
                    t.id_column,
                    t.Title,
                    t.Ketqua,
                    t.Mota,
                    t.CreatedAt,
                    t.UpdateAt,
                    t.Deadline
                })
                .ToList();

            if (!projectTasks.Any())
            {
                return NotFound($"No tasks found for project ID {projectId}");
            }

            return Ok(projectTasks);
        }


        // Sau khi chọn checkbox 
        [HttpGet("tasks/progress")]
            public IActionResult GetTasksWithProgress(int projectId)
            {
                var tasks = _context.Tasks
                    .Where(t => t.IdColumnNavigation.IdProject == projectId) // Lọc theo dự án
                    .Select(t => new
                    {
                        TaskId = t.id_task,
                        Title = t.Title,
                        IsCompleted = t.Ketqua == "Completed", // Hoàn thành nếu trạng thái là "Completed"
                        Deadline = t.Deadline
                    })
                    .ToList();

                var totalTasks = tasks.Count;
                var completedTasks = tasks.Count(t => t.IsCompleted);
                var progress = totalTasks > 0 ? (completedTasks * 100 / totalTasks) : 0;

                return Ok(new
                {
                    Progress = progress, // Tỷ lệ phần trăm hoàn thành
                    Tasks = tasks
                });
            }

        [HttpPut]
        [AllowAnonymous]
        [Route("UpdateTrangThai")]
        public async Task<ActionResult> UpdateTrangThai(int id_task, bool ck)
        {
            try
            {
                // Tìm task theo id_task
                var task = _context.Tasks.FirstOrDefault(t => t.id_task == id_task);
                if (task == null)
                {
                    // Nếu không tìm thấy task, trả về lỗi 404
                    return NotFound(new { error = $"Task with ID {id_task} not found." });
                }

                // Cập nhật trạng thái công việc dựa trên giá trị của ck
                if (ck)  // Nếu checkbox được chọn (true)
                {
                    if (task.Ketqua == null || !task.Ketqua.Contains("Hoàn thành")) // Kiểm tra nếu Ketqua là null hoặc chưa có "Hoàn thành"
                    {
                        task.Ketqua = (task.Ketqua ?? "") + " - Hoàn thành";
                    }
                }
                else // Nếu checkbox không được chọn (false)
                {
                    if (task.Ketqua != null && task.Ketqua.Contains("Hoàn thành")) // Nếu có "Hoàn thành", xóa nó
                    {
                        task.Ketqua = task.Ketqua.Replace(" - Hoàn thành", "");
                    }
                }

                // Cập nhật thời gian sửa đổi
                task.UpdateAt = DateTime.Now;

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                // Trả về phản hồi thành công với thông tin cập nhật
                return Ok(new
                {
                    message = "Task result updated successfully.",
                    updatedTask = new
                    {
                        task.id_task,
                        task.Title,
                        task.Ketqua,
                        task.UpdateAt
                    }
                });
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có ngoại lệ xảy ra
                return StatusCode(500, new { error = "An error occurred while updating the task: " + ex.Message });
            }
        }
    }

}
