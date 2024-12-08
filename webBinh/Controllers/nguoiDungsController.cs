using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using webBinh.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using webBinh.LoginReponse;
using Newtonsoft.Json;
using System.IO;



namespace webBinh.Controllers
{
    public class nguoiDungsController : Controller
    {
        private readonly HttpClient _httpClient;
        private webNangCaoQLDA2Entities1 db = new webNangCaoQLDA2Entities1();
        List<webBinh.Models.Task> task = new List<webBinh.Models.Task>();
        List<webBinh.Models.nguoiDung> nguoidung = new List<webBinh.Models.nguoiDung>();
        public nguoiDungsController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7217/");  // Đảm bảo sử dụng API base URL chính xác
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(string idUser, string pass)
        {
            // Kiểm tra dữ liệu nhập vào
            if (string.IsNullOrWhiteSpace(idUser) || string.IsNullOrWhiteSpace(pass))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập đầy đủ thông tin đăng nhập.";
                return View("Index");
            }

            // Tạo đối tượng DTO gửi lên API
            var loginDto = new { IdUser = idUser, Pass = pass };


            // cookie
            HttpCookie loginCookie = new HttpCookie("LoginCookie");
            loginCookie.Value = idUser; // Lưu tên đăng nhập
            loginCookie.Expires = DateTime.Now.AddHours(1); // Cookie có hiệu lực trong 1 giờ
            Response.Cookies.Add(loginCookie);
            try
            {
                // Gửi yêu cầu POST đến API
                var response = await _httpClient.PostAsJsonAsync("NguoiDung/Login", loginDto);

                if (response.IsSuccessStatusCode)
                {
                    // Đọc kết quả trả về
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);

                    // Lưu token và thông tin user vào session
                    Session["Token"] = loginResponse.Token;
                    Session["UserId"] = loginResponse.User.id_user;

                    // Chuyển hướng đến trang dự án
                    //return RedirectToAction("index", "project");
                    return RedirectToAction("index", "project", new { sdt = idUser });

                }
                else
                {
                    ViewBag.ErrorMessage = "Đăng nhập không thành công";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi khi kết nối tới API: {ex.Message}";
            }

            return View("Index"); // Trả về view đăng nhập nếu có lỗi
        }


        // DTO cho response khi login thành công
        public class LoginResponse
        {
            public string Token { get; set; }
            public nguoiDung User { get; set; }
        }





        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register(string IdUser, string email, string password, string username, string fullname)
        {
            if (string.IsNullOrWhiteSpace(IdUser) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(fullname))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập đầy đủ thông tin.";
                return View("Register");
            }

            string fileName = null;
            if (Request.Files.Count > 0)
            {
                var avatarFile = Request.Files[0];
                if (avatarFile != null && avatarFile.ContentLength > 0)
                {
                     fileName = Path.GetFileName(avatarFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/IMG"), fileName);
                    avatarFile.SaveAs(path);
                }
            }

            var nguoiDungDTO = new
            {
                IdUser = IdUser,
                Email = email,
                Pass = password,
                Username = username,
                Fullname = fullname,
                Avatar = fileName, // Gán fileName từ bước xử lý file
                IdRole = "1",
                IsActive = true,
            };

            try
            {
                var jsonContent = new StringContent(JsonConvert.SerializeObject(nguoiDungDTO), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("NguoiDung/Register", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.SuccessMessage = "Đăng ký thành công! Vui lòng đăng nhập.";
                    return RedirectToAction("Index", "nguoiDungs");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ViewBag.ErrorMessage = !string.IsNullOrWhiteSpace(errorContent)
                        ? "Đăng ký không thành công: " + errorContent
                        : "Đăng ký không thành công. Vui lòng thử lại.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi khi kết nối tới API: {ex.Message}";
            }

            return View("Register");
        }




        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }





































        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        // GET: nguoiDungs
        public ActionResult Index()
        {
            var nguoiDungs = db.nguoiDungs.Include(n => n.vaiTro);
            return View("Index");
        }

        // GET: nguoiDungs/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            nguoiDung nguoiDung = db.nguoiDungs.Find(id);
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }
            return View(nguoiDung);
        }

        // GET: nguoiDungs/Create
        public ActionResult Create()
        {
            ViewBag.id_role = new SelectList(db.vaiTroes, "id_role", "role_name");
            return View();
        }

        // POST: nguoiDungs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_user,email,pass,username,avatar,id_role,isActive,fullname")] nguoiDung nguoiDung)
        {
            if (ModelState.IsValid)
            {
                db.nguoiDungs.Add(nguoiDung);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_role = new SelectList(db.vaiTroes, "id_role", "role_name", nguoiDung.id_role);
            return View(nguoiDung);
        }

        // GET: nguoiDungs/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            nguoiDung nguoiDung = db.nguoiDungs.Find(id);
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_role = new SelectList(db.vaiTroes, "id_role", "role_name", nguoiDung.id_role);
            return View(nguoiDung);
        }

        // POST: nguoiDungs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_user,email,pass,username,avatar,id_role,isActive,fullname")] nguoiDung nguoiDung)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nguoiDung).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_role = new SelectList(db.vaiTroes, "id_role", "role_name", nguoiDung.id_role);
            return View(nguoiDung);
        }

        // GET: nguoiDungs/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            nguoiDung nguoiDung = db.nguoiDungs.Find(id);
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }
            return View(nguoiDung);
        }

        // POST: nguoiDungs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            nguoiDung nguoiDung = db.nguoiDungs.Find(id);
            db.nguoiDungs.Remove(nguoiDung);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
