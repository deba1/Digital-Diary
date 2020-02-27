using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using DigitalDiary.Models;

namespace DigitalDiary.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        DiaryDbContext db = new DiaryDbContext();

        public ActionResult Index()
        {
            if (Session["username"] != null)
                return RedirectToAction("Index", "Diary");
            else
                return RedirectToAction("Login", "Account");
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (Session["username"] != null)
                return RedirectToAction("Index", "Diary");
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (Session["username"] != null)
                return RedirectToAction("Index", "Diary");
            User user = db.Users.Where(u => u.Username == username && u.Password == password).FirstOrDefault();

            if (user != null)
            {
                Session["user_id"] = user.Id;
                Session["username"] = user.Username;
                return RedirectToAction("Index", "Diary");
            }
            else
            {
                TempData["Error"] = "Username/Password is Incorrect";
                return RedirectToAction("Login");
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (Session["username"] != null)
                return RedirectToAction("Index", "Diary");
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    TempData["New"] = "Account Successfully Created. Please proceed to login.";
                    return RedirectToAction("Login");
                }
            }
            catch (Exception e)
            {
                TempData["Error"] = "Username Already Exist!";
            }
            return View(user);
        }

     
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Account");
        }

    }
}