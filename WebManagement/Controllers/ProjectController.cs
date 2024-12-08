using APIwebmoi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;
namespace API.Controllers
{
    public class ProjectController : Controller
    {
        private readonly WebNangCaoQlda2Context _context;
        public ProjectController(WebNangCaoQlda2Context context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("GetTenProject")]
        public async Task<ActionResult<IEnumerable<Project>>> GetTenProject(string sdt)
        {
            var k = await _context.Projects.Where(i => i.OwnerIds == sdt).ToListAsync();

            return k;
        }

        [HttpGet]
        [Route("GetColumn")]
        public async Task<ActionResult<IEnumerable<Column>>> GetColumn(int id_project)
        {
            var y = await _context.Columns.Where(i => i.IdProject == id_project).ToListAsync();

            return y;
        }
       

        [HttpGet]
        [Route("GetTask")]
        public async Task<ActionResult<IEnumerable<APIwebmoi.Models.Task>>> GetTask(string sdt)
        {
            // Lấy danh sách id_task từ UserTask mà id_user khớp với sdt
            var taskIds = await _context.UserTasks
                                        .Where(i => i.IdUser == sdt)
                                        .Select(i => i.IdTask)
                                        .ToListAsync();

            // Lấy danh sách Task với id_task có trong danh sách taskIds và id_column khớp với tham số id_column
            var tasks = await _context.Tasks
                                      .Where(i => taskIds.Contains(i.id_task))
                                      .ToListAsync();

            return tasks;
        }
        [HttpPost("createPro")]

        public async Task<ActionResult> createPro(APIwebmoi.Models.Project pr)
        {
            // Lấy danh sách id_task từ UserTask mà id_user khớp với sdt
            _context.Projects.Add(pr);
            await _context.SaveChangesAsync();


            return Ok(new { mesage = "ok" });
        }
        [HttpPost("createColumn")]

        public async Task<ActionResult> createColumn(APIwebmoi.Models.Column column)
        {
            // Lấy danh sách id_task từ UserTask mà id_user khớp với sdt
            _context.Columns.Add(column);
            await _context.SaveChangesAsync();


            return Ok(new { mesage = "ok" });
        }

        //cap nhat project
        [HttpPut("updatePro")]
        public async Task<ActionResult> updatePro(APIwebmoi.Models.Project updatedProject)
        {
            var existingProject = await _context.Projects.FindAsync(updatedProject.id_project);

            if (existingProject == null)
            {
                return NotFound(new { message = "Project not found" });
            }

            existingProject.Mota = updatedProject.Mota;
            existingProject.Title = updatedProject.Title;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Project updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the project", details = ex.Message });
            }
        }

        //xoa project
        [HttpDelete("deleteProject/{id_project}")]
        public async Task<ActionResult> DeleteProject(int id_project)
        {
            // Tìm project cần xóa
            var project = await _context.Projects.FindAsync(id_project);

            // Kiểm tra nếu không tìm thấy
            if (project == null)
            {
                return NotFound(new { message = "Project không tồn tại." });
            }

            // Xóa project
            _context.Projects.Remove(project);

            // Lưu thay đổi
            await _context.SaveChangesAsync();

            // Trả về kết quả
            return Ok(new { message = "Xóa project thành công." });
        }
        
        [HttpGet("getidProjectByidColumn/{id_column}")]
        public async Task<ActionResult> getidProjectByidColumn(int id_column)
        {
            // Tìm project cần xóa
            var cl = await _context.Columns.FindAsync(id_column);

            // Kiểm tra nếu không tìm thấy
            if (cl == null)
            {
                return NotFound(new { message = "Project không tồn tại." });
            }
            // Trả về kết quả
            return Ok(cl.IdProject);
        }


        [HttpGet("getmaxIdPro")]
        public async Task<ActionResult> getmaxIdPro()
        {
            // Lấy danh sách id_task từ UserTask mà id_user khớp với sdt

            int maxId = _context.Projects.Max(i => i.id_project);
            int idpr = maxId + 1;
            return Ok(new { mesage = idpr });
        }

        [HttpGet("getidPro")]
        public async Task<ActionResult> getidPro(int idcl)
        {
            // Lấy id_project từ bảng Columns
            var k = await _context.Columns
                                  .Where(i => i.id_column == idcl)
                                  .Select(c => c.IdProject) // Đảm bảo thuộc tính idProject tồn tại
                                  .FirstOrDefaultAsync();

            return Ok(new { message = k });
        }


        [HttpGet("getmaxIdCl")]
        public async Task<ActionResult> getmaxIdCl()
        {
            // Lấy danh sách id_task từ UserTask mà id_user khớp với sdt

            int maxId = _context.Columns.Max(i => i.id_column);
            int idpr = maxId + 1;
            return Ok(new { mesage = idpr });
        }

        [HttpGet]
        [Route("GetColumnS")]
        public async Task<ActionResult<Column>> GetColumnS(int idcolumn)
        {
            var y = await _context.Columns.Where(i => i.id_column == idcolumn).FirstOrDefaultAsync();
            return y;
        }
        
        [HttpPut]
        [Route("updateColumn")]
        public async Task<ActionResult<Column>> updateColumn(int idcl, string tt)
        {
            var y = await _context.Columns.Where(i => i.id_column == idcl).FirstOrDefaultAsync();
            y.Title = tt;
            await _context.SaveChangesAsync();

            return y;
        }


        [HttpDelete("deleteColumn/{idColumn}")]
        [AllowAnonymous]
        public async Task<ActionResult> DeleteColumn(int idColumn)
        {
            var existingColumn = _context.Columns
                .FirstOrDefault(x => x.id_column == idColumn);

            if (existingColumn == null)
            {
                return NotFound(new { Message = "Task not found." });
            }

            _context.Columns.Remove(existingColumn);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Column deleted successfully." });
        }
    }
}