using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using webBinh.Models;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Text.Json;

namespace webBinh.Controllers
{
    public class projectController : Controller
    {
        // GET: project
        //string sdt = "0839976113";
        private webNangCaoQLDA2Entities1 db = new webNangCaoQLDA2Entities1();
        List<webBinh.Models.Task> task = new List<webBinh.Models.Task>();
        List<webBinh.Models.Project> project = new List<webBinh.Models.Project>();
     

        public ActionResult Index(string sdt)
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync("https://localhost:7217/GetTenProject?sdt=" + sdt).Result; // Thay URL bằng URL API của bạn

                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;

                    project = JsonConvert.DeserializeObject<List<Project>>(responseData);
                }
                else
                {
                    ViewBag.ErrorMessage = "Không thể kết nối với API.";
                }
            }
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync("https://localhost:7217/NguoiDung/Profile?sdt=" + sdt).Result; // Thay URL bằng URL API của bạn

                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;

                    nguoiDung x = JsonConvert.DeserializeObject<nguoiDung>(responseData);
                    ViewBag.avatar = x.avatar;
                }
                else
                {
                    ViewBag.ErrorMessage = "Không thể kết nối với API.";
                }
            }
            return View(project.ToList());
        }
        
        public  ActionResult column(int id_project, string sdt)
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync("https://localhost:7217/GetTenProject?sdt=" + sdt).Result; // Thay URL bằng URL API của bạn

                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;

                    project = JsonConvert.DeserializeObject<List<Project>>(responseData);
                }
                else
                {
                    ViewBag.ErrorMessage = "Không thể kết nối với API.";
                }
            }
            ViewBag.project = project.ToList();
            
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync("https://localhost:7217/NguoiDung/Profile?sdt=" + sdt).Result; // Thay URL bằng URL API của bạn

                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;

                    nguoiDung x = JsonConvert.DeserializeObject<nguoiDung>(responseData);
                    ViewBag.avatar = x.avatar;
                }
                else
                {
                    ViewBag.ErrorMessage = "Không thể kết nối với API.";
                }
            }
            


            var ten= project.Where(i=>i.id_project==id_project).FirstOrDefault();
            ViewBag.ten = "Dự án";
            ViewBag.id=id_project;


            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync("https://localhost:7217/GetTask?sdt=" + sdt).Result; // Thay URL bằng URL API của bạn

                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;

                    task = JsonConvert.DeserializeObject<List<webBinh.Models.Task>>(responseData);
                }
                else
                {
                    ViewBag.ErrorMessage = "Không thể kết nối với API.";
                }
            }
            ViewBag.task=task.ToList();


            List<webBinh.Models.column> columns = new List<webBinh.Models.column>();
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync("https://localhost:7217/GetColumn?id_project=" + id_project).Result; // Thay URL bằng URL API của bạn

                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;

                    columns = JsonConvert.DeserializeObject<List<webBinh.Models.column>>(responseData);
                }

            }
            return View(columns.ToList());
        }

        public async Task<ActionResult> createProject([Bind(Include = "id_project,mota,title,ownerIds,createdAt")] Project project)
        {
            int idmax;
            using (var httpClient = new HttpClient())
            {
                var url = "https://localhost:7217/createPro";
                var content = new FormUrlEncodedContent(new[]
                {
    new KeyValuePair<string, string>("id_project", project.id_project.ToString()),
    new KeyValuePair<string, string>("Mota", project.mota),
    new KeyValuePair<string, string>("Title", project.title),
    new KeyValuePair<string, string>("OwnerIds", project.ownerIds),
    new KeyValuePair<string, string>("CreatedAt", project.createdAt.ToString())
});

                var response = await httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var loginCookie = Request.Cookies["LoginCookie"];
                    if (responseData.Contains("ok"))
                    {
                        return RedirectToAction("index", new { sdt = loginCookie.Value });
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Không thể kết nối với API.";
                }
            }
            using (var httpClient = new HttpClient())
            {
                var url = "https://localhost:7217/getmaxIdPro";

                var response = await httpClient.GetAsync(url);
                var responseData = await response.Content.ReadAsStringAsync();
                //idmax = responseData;
                var jsonDoc = JsonDocument.Parse(responseData);
                idmax = jsonDoc.RootElement.GetProperty("mesage").GetInt32();
                ViewBag.id = idmax.ToString();
                project.id_project = idmax;
            }

            return View(project);
        }



        //get prj to update
        public async Task<ActionResult> EditProject(int id)
        {
            var project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: project/editProject
        [HttpPost]
        public async Task<ActionResult> EditProject(Project project)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var httpClient = new HttpClient())
                    {


                        var url = $"https://localhost:7217/updatePro?id_project={project.id_project}&Mota={project.mota}&Title={project.title}";
                        var emptyContent = new StringContent("", System.Text.Encoding.UTF8, "application/json");

                        var response = await httpClient.PutAsync(url, emptyContent);
                        if (response.IsSuccessStatusCode)
                        {
                             var loginCookie = Request.Cookies["LoginCookie"];
                            return RedirectToAction("index", new { sdt = loginCookie.Value });
                        }
                        else
                        {
                            ModelState.AddModelError("", "Không thể cập nhật dự án.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi cập nhật: {ex.Message}");
                }
            }
            return View(project);
        }


        //del prj with id
        [HttpPost]
        public async Task<ActionResult> DeleteProject(int id_project)
        {
            try
            {
                // Gửi yêu cầu DELETE đến API
                using (var httpClient = new HttpClient())
                {
                    var apiUrl = $"https://localhost:7217/deleteProject/{id_project}";
                    var response = await httpClient.DeleteAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        // Lưu thông báo thành công
                        TempData["SuccessMessage"] = "Xóa project thành công.";
                    }
                    else
                    {
                        // Lưu thông báo lỗi
                        var errorContent = await response.Content.ReadAsStringAsync();
                        TempData["ErrorMessage"] = $"Không thể xóa project: {errorContent}";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xóa: {ex.Message}";
            }

            // Quay lại trang Index sau khi xử lý
            return RedirectToAction("Index");
        }

        ///
        public async Task<ActionResult> GetProjectProgress(int idProject)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    string apiUrl = $"https://localhost:7217/tasks/{idProject}";
                    var response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var tasks = JsonConvert.DeserializeObject<List<Models.Task>>(responseData);

                        if (tasks == null)
                        {
                            tasks = new List<Models.Task>();
                        }

                        ViewBag.TotalTasks = tasks.Count;
                        ViewBag.HoanThanh = tasks.Count(t => !string.IsNullOrEmpty(t.Ketqua) && t.Ketqua.Contains("- Hoàn thành"));
                        ViewBag.DangThucHien = tasks.Count(t => t.updateAt != null && t.deadline != null && t.deadline < DateTime.Now && !string.IsNullOrEmpty(t.Ketqua) && !t.Ketqua.Contains("- Hoàn thành"));
                        ViewBag.ChuaBatDau = tasks.Count(t => t.deadline == null);
                        ViewBag.QuaHan = tasks.Count(t => t.deadline != null && t.deadline < DateTime.Now && !string.IsNullOrEmpty(t.Ketqua) && !t.Ketqua.Contains("- Hoàn thành"));
                    }
                    else
                    {
                        ViewBag.ErrorMessage = $"Lỗi API: {response.StatusCode} - {response.ReasonPhrase}";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Đã xảy ra lỗi: {ex.Message}";
            }

            return View("ProgressChart");
        }


        public ActionResult GetTaskByProject(int idProject)
        {
            List<dynamic> tasksWithMembers = new List<dynamic>();

            try
            {
                using (var httpClient = new HttpClient())
                {
                    string apiUrl = $"https://localhost:7217/tasks-by-project/{idProject}";
                    var response = httpClient.GetAsync(apiUrl).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        tasksWithMembers = JsonConvert.DeserializeObject<List<dynamic>>(responseData);

                        Console.WriteLine("Dữ liệu trả về từ API: ");
                        foreach (var task in tasksWithMembers)
                        {
                            Console.WriteLine($"Task: {task.TaskName}, Members: {task.Members}");
                        }


                        foreach (var task in tasksWithMembers)
                        {
                            if (task.Members == null)
                            {
                                task.Members = new List<string>(); // Hoặc kiểu danh sách khác phù hợp
                            }
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessage = $"Lỗi API: {response.StatusCode} - {response.ReasonPhrase}";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Đã xảy ra lỗi: {ex.Message}";
            }

            ViewBag.taskMember = tasksWithMembers;
            return View("GetTaskByProject");
        }

        public ActionResult GetAllTaskByProject()
        {
            List<dynamic> tasksWithMembers = new List<dynamic>();

            try
            {
                using (var httpClient = new HttpClient())
                {
                    string apiUrl = $"https://localhost:7217/tasks-by-project";
                    var response = httpClient.GetAsync(apiUrl).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        tasksWithMembers = JsonConvert.DeserializeObject<List<dynamic>>(responseData);

                        Console.WriteLine("Dữ liệu trả về từ API: ");
                        foreach (var task in tasksWithMembers)
                        {
                            Console.WriteLine($"Task: {task.TaskName}, Members: {task.Members}");
                        }

                        foreach (var task in tasksWithMembers)
                        {
                            if (task.Members == null)
                            {
                                task.Members = new List<string>();
                            }
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessage = $"Lỗi API: {response.StatusCode} - {response.ReasonPhrase}";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Đã xảy ra lỗi: {ex.Message}";
            }

            ViewBag.allTask = tasksWithMembers;
            return View("GetAllTaskByProject");
        }

    }
}