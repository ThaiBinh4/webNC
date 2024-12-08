using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using webBinh.Models;
using Task = webBinh.Models.Task;

namespace webBinh.Controllers
{
    public class TaskController : Controller
    {
        private webNangCaoQLDA2Entities1 db = new webNangCaoQLDA2Entities1();
        // GET: Task
        public ActionResult Index()
        {
            var task = db.Tasks.Include(c => c.column);
            return View(task.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }

    

        public async Task<ActionResult> CreateTask([Bind(Include = "id_task,title,createdAt")] Task task,string idcl, string iduser)
        {
            ViewBag.idtask = task.id_task;
            ViewBag.cl = idcl;
            ViewBag.tit = task.title;
            ViewBag.tt = iduser;
            if (task.title == null)
            {

                ViewBag.idcl = int.Parse( idcl);
                return View();
            }
            int x=0;
            using (var httpClient = new HttpClient())
            { 
                var url = $"https://localhost:7217/getidProjectByidColumn/{idcl}";

                var emptyContent = new StringContent("", System.Text.Encoding.UTF8, "application/json");

                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    x = int.Parse(responseData);
                }
                else
                {
                    ViewBag.ErrorMessage = "Không thể kết nối với API.";
                }
            }


            using (var httpClient = new HttpClient())
            { 
                var url = $"https://localhost:7217/createTask?id_task={task.id_task}&id_column={idcl}&title={task.title}&id={iduser}";

                var emptyContent = new StringContent("", System.Text.Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(url, emptyContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    if (responseData.Contains("successfully") && task != null)
                    {
                        var loginCookie = Request.Cookies["LoginCookie"];
                        return RedirectToAction("column", "project", new { id_project = x, sdt = loginCookie.Value });
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Không thể kết nối với API.";
                }
               

            }
            return View();
        }

        // GET: Hiển thị form với idTask
        public async Task<ActionResult> TaskDetail(int idTask)
        {
            var task = GetTaskById(idTask); // Lấy thông tin task

            

            using (var httpClient = new HttpClient())
            {
                var url = $"https://localhost:7217/GetTaskById?idtask={idTask}";

                try
                {
                    var response = await httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        task = JsonConvert.DeserializeObject<webBinh.Models.Task>(responseData);
                    }
                    else
                    {
                        // Log lỗi hoặc xử lý lỗi API
                        Console.WriteLine($"API trả về lỗi: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    // Log lỗi khi gọi API
                    Console.WriteLine($"Lỗi khi gọi API: {ex.Message}");
                }
            }
         
            ViewBag.idTask = task.id_task;
            ViewBag.idTt = task.title;

            return View(task);
        }

        private Task GetTaskById(int idTask)
        {
            webBinh.Models.Task x= new webBinh.Models.Task();
            using (var db = new webNangCaoQLDA2Entities1())
            {
                // Tìm task theo id
                using (var httpClient = new HttpClient())
                {
                    var url = $"https://localhost:7217/GetTaskById?idtask="+idTask;

                    var response = httpClient.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        x = JsonConvert.DeserializeObject<webBinh.Models.Task>(responseData);
                    }
                }

                
            }
            return x;
        }


        // POST: Xử lý submit form
        [HttpPost]
        public async Task<ActionResult> TaskDetail(int idTask, Task task)
        {
            if (task == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid task data.");
            }

            // Lấy id_column từ task nếu cần thiết
            int? idColumn = task.id_column; // Giả sử task có thuộc tính id_column

            var title = task.title ?? string.Empty;
            var mota = task.mota ?? string.Empty;
            var ketqua = task.Ketqua ?? string.Empty;
        

            // Tạo URL với các tham số
            var url = $"https://localhost:7217/updateTask/{idTask}?mota={task.mota}&ketqua={task.Ketqua}&deadline={task.deadline}";

            // Logic xử lý cập nhật task
            using (var httpClient = new HttpClient())
            {
                var jsonContent = new StringContent(
                    Newtonsoft.Json.JsonConvert.SerializeObject(task),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var response = await httpClient.PutAsync(url, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    if (responseData.Contains("successfully"))
                    {
                        return RedirectToAction("TaskDetail", new { idTask = idTask }); // Quay lại trang chi tiết task
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Không thể kết nối với API.";
                }
            }

            return View(task);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteTask(int idTask)
        {
            if (idTask <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid task ID.");
            }

            using (var httpClient = new HttpClient())
            {
                try
                {
                    // API endpoint xóa task
                    var url = $"https://localhost:7217/deleteTask/{idTask}";

                    // Gửi yêu cầu xóa
                    var response = await httpClient.DeleteAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();

                        // Kiểm tra dữ liệu trả về từ API
                        if (responseData.Contains("successfully"))
                        {
                            TempData["SuccessMessage"] = "Task deleted successfully.";

                            // Lấy project ID sau khi xóa (giả sử bạn có thông tin này từ API hoặc dữ liệu khác)
                            int x = 0; // Lấy project ID từ dữ liệu khác nếu cần (ví dụ từ responseData hoặc database)

                            var loginCookie = Request.Cookies["LoginCookie"];
                            return RedirectToAction("column", "project", new { id_project = x, sdt = loginCookie.Value });
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Không thể xóa task. API trả về lỗi.";
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Không thể xóa task. API trả về lỗi.";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Đã xảy ra lỗi khi kết nối tới API: " + ex.Message;
                }
            }
            ViewBag.ten = "Dự án";
            return View();
        }




        // Tan Quang Huy
        [HttpPut]
        [AllowAnonymous]
        public async Task<ActionResult> UpdateTrangThai(int idTask, bool ck)
        {
            try
            {
                if (idTask <= 0)
                {
                    return new HttpStatusCodeResult(400, "Invalid task ID.");
                }

                var url = $"https://localhost:7217/UpdateTrangThai?id_task={idTask}&ck={ck}";

                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.PutAsync(url, null).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        return Json(new { success = true, data = responseData });
                    }
                    else
                    {
                        return Json(new { success = false, error = $"API call failed with status code: {response.StatusCode}" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }


    }
}