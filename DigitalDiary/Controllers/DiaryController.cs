using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DigitalDiary.Models;
using System.IO;

namespace DigitalDiary.Controllers
{
    public class DiaryController : Controller
    {
        private DiaryDbContext db = new DiaryDbContext();

        // GET: /Diary/
        public ActionResult Index()
        {
            if (Session["username"] == null)
                return RedirectToAction("Index", "Account");
            string user = Session["username"].ToString();
            var diaries = db.Diaries.Include(d => d.Importance1).Include(d => d.User).Where(u=>u.User.Username == user).OrderByDescending(d=>d.LastModification);
            return View(diaries.ToList());
        }

        // GET: /Diary/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["username"] == null)
                return RedirectToAction("Index", "Account");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Diary diary = db.Diaries.Find(id);
            if (diary == null)
            {
                return HttpNotFound();
            }
            return View(diary);
        }

        // GET: /Diary/Create
        public ActionResult Create()
        {
            if (Session["username"] == null)
                return RedirectToAction("Index", "Account");
            ViewBag.Importance = new SelectList(db.Importances, "Id", "Importance1");
            ViewBag.UserId = new SelectList(db.Users, "Id", "Username");
            return View();
        }

        // POST: /Diary/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Diary diary)
        {
            if (Session["username"] == null)
                return RedirectToAction("Index", "Account");
            if (ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                {
                    var postedFile = Request.Files[0];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        string imagesPath = HttpContext.Server.MapPath("~/Content/Images");
                        string extension = Path.GetExtension(postedFile.FileName);
                        string newFileName = Session["username"] + "_" + System.DateTime.Now.ToString("yyMMddHHmmss") + extension;
                        string saveToPath = Path.Combine(imagesPath, newFileName);
                        diary.Image = newFileName;
                        postedFile.SaveAs(saveToPath);
                    }
                }
                
                diary.UserId = (int)Session["user_id"];
                diary.LastModification = System.DateTime.Now;
                db.Diaries.Add(diary);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Importance = new SelectList(db.Importances, "Id", "Importance1", diary.Importance);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Username", diary.UserId);
            return View(diary);
        }

        // GET: /Diary/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["username"] == null)
                return RedirectToAction("Index", "Account");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Diary diary = db.Diaries.Find(id);
            if (diary == null)
            {
                return HttpNotFound();
            }
            ViewBag.Importance = new SelectList(db.Importances, "Id", "Importance1", diary.Importance);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Username", diary.UserId);
            return View(diary);
        }

        // POST: /Diary/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Title,Importance,Date,Note,Image,LastModification,UserId")] Diary diary)
        {
            if (Session["username"] == null)
                return RedirectToAction("Index", "Account");
            if (ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                {
                    var postedFile = Request.Files[0];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        string imagesPath = HttpContext.Server.MapPath("~/Content/Images");
                        string extension = Path.GetExtension(postedFile.FileName);
                        string newFileName = Session["username"] + "_" + System.DateTime.Now.ToString("yyMMddHHmmss") + extension;
                        string saveToPath = Path.Combine(imagesPath, newFileName);
                        diary.Image = newFileName;
                        postedFile.SaveAs(saveToPath);
                    }
                }
                diary.UserId = (int)Session["user_id"];
                diary.LastModification = System.DateTime.Now;
                db.Entry(diary).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Importance = new SelectList(db.Importances, "Id", "Importance1", diary.Importance);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Username", diary.UserId);
            return View(diary);
        }

        // GET: /Diary/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["username"] == null)
                return RedirectToAction("Index", "Account");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Diary diary = db.Diaries.Find(id);
            if (diary == null)
            {
                return HttpNotFound();
            }
            return View(diary);
        }

        // POST: /Diary/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["username"] == null)
                return RedirectToAction("Index", "Account");
            Diary diary = db.Diaries.Find(id);
            db.Diaries.Remove(diary);
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
