using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AplicationNortwind.Models;
using System.Data.Entity;
using System.Net;
using System.Web.Security;
using System.Web.UI.WebControls;
using AplicationNortwind.Filters;
using AplicationNortwind.Utils;

namespace AplicationNortwind.Controllers
{
    [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
    [Authorize]
    public class HomeController : Controller
    {
        private  DataAccess db = new DataAccess();

        // GET: Home
        public ActionResult Index()
        {
            return RedirectToAction("Index","Orders");
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            try
            {
                if (Request.Cookies["AuthID"].Value == Session["AuthID"].ToString())
                {
                    return RedirectToAction("Index", "Orders");
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                return View();
            }

        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(Employees dto)
        {
            IQueryable<Employees> employees = null;
            if (dto.UsernameApp == null || dto.PasswordApp == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }            
            var existeUsuario = db.Employees.Include(e => e.Employees2).Where(p => p.UsernameApp == dto.UsernameApp);

            if (ModelState.IsValid)
            {
                if (existeUsuario.SingleOrDefault() == null)
                {
                    ModelState.AddModelError("", "El usuario no existe");
                    return View(dto);
                }
                dto.PasswordApp = EncriptersUtil.MD5(dto.PasswordApp);
                employees = db.Employees.Include(e => e.Employees2).Where(p => p.UsernameApp == dto.UsernameApp && p.PasswordApp == dto.PasswordApp);
                if (employees.SingleOrDefault() == null)
                {
                    ModelState.AddModelError("", "La contraseña es incorrecta");
                    return View(dto);
                }
            }
            
            FormsAuthentication.SetAuthCookie(dto.EmployeeID.ToString(), false);
            var singleOrDefault = employees.SingleOrDefault();
            if (singleOrDefault != null) Session["Usuario"] = singleOrDefault.UsernameApp;
            string authId = dto.EmployeeID.ToString();
            Session["AuthID"] = authId;
            var cookie = new HttpCookie("AuthID") {Value = authId};
            Response.Cookies.Add(cookie);
            return RedirectToAction("Index","Orders");
        }

        public ActionResult Logout()
        {
            Session["MensajeRegistro"] = null;
            FormsAuthentication.SignOut();
            Session["Usuario"] = null;
            Session.Clear();
            Session.Abandon();
            Session.Abandon();
            return RedirectToAction("Login", "Home");
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