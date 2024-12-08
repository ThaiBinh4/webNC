using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using webBinh.Models;

namespace webBinh.Controllers
{
    public class columnsController : Controller
    {
        private webNangCaoQLDA2Entities1 db = new webNangCaoQLDA2Entities1();
        public column cl = new column();
        // GET: columns
        public ActionResult Index()
        {
            var columns = db.columns.Include(c => c.Project);
            return View(columns.ToList());
        }

        // GET: columns/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            column column = db.columns.Find(id);
            if (column == null)
            {
                return HttpNotFound();
            }
            return View(column);
        }

        // GET: columns/Create
       
        public async Task<ActionResult> CreateColumn([Bind(Include = "id_column,title,createdAt")] column column, string idpr)
        {
            if (column.title == null)
                return View();
            int idmax;
            using (var httpClient = new HttpClient())
                {
                    var url = "https://localhost:7217/getmaxIdCl";

                    var response = await httpClient.GetAsync(url);
                    var responseData = await response.Content.ReadAsStringAsync();
                    
                    var jsonDoc = JsonDocument.Parse(responseData);
                    idmax = jsonDoc.RootElement.GetProperty("mesage").GetInt32();
                    ViewBag.id = idmax.ToString();
                    column.id_column = idmax;
                }
            using (var httpClient = new HttpClient())
                {

                    var url = "https://localhost:7217/createColumn";
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("id_column", column.id_column.ToString()),
                        new KeyValuePair<string, string>("IdProject",idpr),
                        new KeyValuePair<string, string>("Title", column.title),
                        new KeyValuePair<string, string>("CreatedAt", column.createdAt.ToString())
                    });

                    var response = await httpClient.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();

                        if (responseData.Contains("ok"))
                        {
                        var loginCookie = Request.Cookies["LoginCookie"];
                        return RedirectToAction("column", "project", new { id_project=idpr, sdt = loginCookie.Value });
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Không thể kết nối với API.";
                    }
                }
            
                
            return View();
        }

        public async Task<ActionResult> EditColumn(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "ID không hợp lệ");
            }
            column x = new column();

            // Giả sử bạn lấy dữ liệu từ API hoặc database

            int idpr;
            using (var httpClient = new HttpClient())
            {
                var url = "https://localhost:7217/getidPro?idcl=" + id;

                var response = await httpClient.GetAsync(url);
                var responseData = await response.Content.ReadAsStringAsync();

                var jsonDoc = JsonDocument.Parse(responseData);
                idpr = jsonDoc.RootElement.GetProperty("message").GetInt32();


            }

            using (var httpClient = new HttpClient())
            {
                var url = $"https://localhost:7217/GetColumnS?idcolumn={id}";

                var response = httpClient.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    x = JsonConvert.DeserializeObject<column>(responseData);

                }
                else
                {
                    ViewBag.ErrorMessage = "Không thể lấy thông tin column.";
                    return RedirectToAction("Index"); // Hoặc hành động khác nếu không lấy được dữ liệu
                }
            }
            x.id_project = idpr;
            // Truyền dữ liệu vào view
            return View(x);
        }
        [HttpPost]
        public async Task<ActionResult> EditColumn(int id_column, string title, int id_project)
        {
            ViewBag.tt=title;


                using (var httpClient = new HttpClient())
                {
                    var url = $"https://localhost:7217/updateColumn?idcl={id_column}&tt={title}";
                    var emptyContent = new StringContent("", System.Text.Encoding.UTF8, "application/json");

                    var response = await httpClient.PutAsync(url, emptyContent);

                    if (response.IsSuccessStatusCode)
                    {
                    var loginCookie = Request.Cookies["LoginCookie"];
                        return RedirectToAction("column", "project", new { id_project= id_project, sdt = loginCookie.Value});
                    }
                }
                ViewBag.ErrorMessage = "Không thể cập nhật column.";
            

            return View();
        }

        // GET: columns/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            column column = db.columns.Find(id);
            if (column == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_project = new SelectList(db.Projects, "id_project", "mota", column.id_project);
            return View(column);

        }

        // POST: columns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_column,id_project,title,createdAt")] column column)
        {
            if (ModelState.IsValid)
            {
                db.Entry(column).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_project = new SelectList(db.Projects, "id_project", "mota", column.id_project);
            return View(column);
        }

        // GET: columns/Delete/5
        public ActionResult Delete(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var httpClient = new HttpClient())
            {
                var url = $"https://localhost:7217/GetColumnS?idcolumn={id}";

                var response = httpClient.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    cl = JsonConvert.DeserializeObject<column>(responseData);

                }
                else
                {
                    ViewBag.ErrorMessage = "Không thể lấy thông tin column.";
                    return RedirectToAction("Index"); // Hoặc hành động khác nếu không lấy được dữ liệu
                }
            }

            if (cl == null)
            {
                ViewBag.ErrorMessage = "Column không tồn tại.";
                return RedirectToAction("Index");
            }
            return View(cl);
        }


        // POST: columns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            int idpr;
            using (var httpClient = new HttpClient())
            {
                var url = "https://localhost:7217/getidPro?idcl="+id;

                var response = await httpClient.GetAsync(url);
                var responseData = await response.Content.ReadAsStringAsync();

                var jsonDoc = JsonDocument.Parse(responseData);
                idpr = jsonDoc.RootElement.GetProperty("message").GetInt32();
               
             
            }


            using (var httpClient = new HttpClient())
            {
                var url = $"https://localhost:7217/deleteColumn/{id}";
                var response = await httpClient.DeleteAsync(url);

                var loginCookieValue = Request.Cookies["LoginCookie"]?.Value;
                
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("column", "project", new { id_project = idpr, sdt = loginCookieValue });
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ViewBag.ErrorMessage = $"Không thể xóa column: {errorMessage}";
                }
            }

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
