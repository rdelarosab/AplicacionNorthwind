using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AplicationNortwind.Models;
using AplicationNortwind.Utils;

namespace AplicationNortwind.Controllers
{
    public class EmployeesController : Controller
    {
        private DataAccess db = new DataAccess();      

        // GET: Employees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UsernameApp,PasswordApp")] Employees employees)
        {
            if (employees.UsernameApp == null || employees.PasswordApp == null)
            {
                ModelState.AddModelError("", "Los campos son obligatorios");
                return View(employees);
            }
            var existeUsuario = db.Employees.Include(e => e.Employees2).Where(p => p.UsernameApp == employees.UsernameApp);

            if (ModelState.IsValid)
            {
                if (existeUsuario.SingleOrDefault() != null)
                {
                    ModelState.AddModelError("", "El usuario ya existe");
                    return View(employees);
                }
                employees.PasswordApp = EncriptersUtil.MD5(employees.PasswordApp);
                Random rd = new Random();
                employees.LastName = "Employeed" + rd.Next(1, 1000).ToString();
                db.Employees.Add(employees);
                db.SaveChanges();
                Session["MensajeRegistro"] = "Usuario registrado correctamente, inicie sesión por favor.";               
                return RedirectToAction("Login","Home");
            }          
            return View(employees);
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
