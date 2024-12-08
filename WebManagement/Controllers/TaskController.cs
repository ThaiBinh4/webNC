using APIwebmoi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebManagement.DTO;

namespace WebManagement.Controllers
{
    public class TaskController : Controller
    {
        private readonly WebNangCaoQlda2Context _context;

        public TaskController(WebNangCaoQlda2Context context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("createTask")]
        [AllowAnonymous]
        public async Task<ActionResult> CreateTask(TaskDTO taskDTO, string id)
        {
            var maxId = _context.Tasks.Max(t => (int?)t.id_task) ?? 0;

            var existingColumn = _context.Columns
                .FirstOrDefault(x => x.id_column == taskDTO.id_column);

            if (existingColumn == null)
            {
                return BadRequest(new { Message = "Column with the given id_column not found." });
            }

            var task = new APIwebmoi.Models.Task
            {
                id_task = maxId + 1,
                id_column = taskDTO.id_column,
                Title = taskDTO.Title,
                CreatedAt = DateTime.Now
            };
            await _context.SaveChangesAsync();

            _context.Tasks.Add(task);
            var uk = new APIwebmoi.Models.UserTask
            {
                IdTask = task.id_task,
                IdUser = id
            };
            _context.UserTasks.Add(uk);
            await _context.SaveChangesAsync();


            return Ok(new { Message = "Task created successfully." });
        }



        [HttpPut("updateTask/{idTask}")]
        [AllowAnonymous]
        public async Task<ActionResult> UpdateTask(int idTask, TaskDTO taskDTO)
        {
            // Tìm task hiện tại theo id_task
            var existingTask = _context.Tasks
                .FirstOrDefault(x => x.id_task == idTask);

            if (existingTask == null)
            {
                return NotFound(new { Message = "Task not found." });
            }

            // Cập nhật các trường nếu không null
            if (!string.IsNullOrEmpty(taskDTO.Title))
            {
                existingTask.Title = taskDTO.Title;
            }

            if (!string.IsNullOrEmpty(taskDTO.Ketqua))
            {
                existingTask.Ketqua = taskDTO.Ketqua;
            }

            if (!string.IsNullOrEmpty(taskDTO.Mota))
            {
                existingTask.Mota = taskDTO.Mota;
            }

            if (taskDTO.id_column.HasValue)
            {
                existingTask.id_column = taskDTO.id_column.Value;
            }

            if (taskDTO.Deadline.HasValue)
            {
                existingTask.Deadline = taskDTO.Deadline.Value;
            }

            // Luôn cập nhật thời gian chỉnh sửa
            existingTask.UpdateAt = DateTime.Now;

            // Lưu thay đổi
            _context.Tasks.Update(existingTask);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Task updated successfully." });
        }

        [HttpDelete("deleteTask/{idTask}")]
        [AllowAnonymous]
        public async Task<ActionResult> DeleteTask(int idTask)
        {
            var existingTask = _context.Tasks
                .Include(t => t.UserTasks)
                .FirstOrDefault(x => x.id_task == idTask);

            if (existingTask == null)
            {
                return NotFound(new { Message = "Task not found." });
            }

            _context.UserTasks.RemoveRange(existingTask.UserTasks);

            _context.Tasks.Remove(existingTask);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Task and related UserTasks deleted successfully." });
        }

        [HttpGet]
        [Route("GetTaskById")]
        public async Task<ActionResult<APIwebmoi.Models.Task>> GetTaskById(int idtask)
        {
            var y = await _context.Tasks.Where(i => i.id_task == idtask).FirstOrDefaultAsync();
            return y;
        }
    }
}
