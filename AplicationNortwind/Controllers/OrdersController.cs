using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AplicationNortwind.Filters;
using AplicationNortwind.Models;
using PagedList;
using AplicationNortwind.Utils;

namespace AplicationNortwind.Controllers
{
    [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
    [Authorize]
    public class OrdersController : Controller
    {
        private DataAccess db = new DataAccess();
        [SecurityCoockie]
        // GET: Orders
        public ActionResult Index(int? page)
        {
            Session["ErrorMsgDeleteCascada"] = null;
            var orders = db.Orders.Include(o => o.Customers).Include(o => o.Employees).Include(o => o.Shippers).OrderByDescending(p=> p.OrderID);
            var pageNumber = page ?? 1;
            var onePageOfOrders = orders.ToList().ToPagedList(pageNumber, 5);
            ViewBag.OnePageOfOrders = onePageOfOrders;
            return View(onePageOfOrders.ToList());
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            var order_Details = db.Order_Details.Include(o => o.Orders).Include(o => o.Products).Where(p => p.OrderID == id);
            var orderIq = db.Orders.Include(o => o.Customers).Include(o => o.Employees).Include(o => o.Shippers).Where(p => p.OrderID == id);
            var orderObj = orderIq.SingleOrDefault();
            Session["OrderSession"] = orderObj;
            Session["idOrdenCBO"] = id;
            return View(order_Details.ToList());
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CompanyName");
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName");
            ViewBag.ShipVia = new SelectList(db.Shippers, "ShipperID", "CompanyName");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,CustomerID,EmployeeID,OrderDate,RequiredDate,ShippedDate,ShipVia,Freight,ShipName,ShipAddress,ShipCity,ShipRegion,ShipPostalCode,ShipCountry")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(orders);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CompanyName", orders.CustomerID);
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName", orders.EmployeeID);
            ViewBag.ShipVia = new SelectList(db.Shippers, "ShipperID", "CompanyName", orders.ShipVia);
            return View(orders);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CompanyName", orders.CustomerID);
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName", orders.EmployeeID);
            ViewBag.ShipVia = new SelectList(db.Shippers, "ShipperID", "CompanyName", orders.ShipVia);
            return View(orders);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,CustomerID,EmployeeID,OrderDate,RequiredDate,ShippedDate,ShipVia,Freight,ShipName,ShipAddress,ShipCity,ShipRegion,ShipPostalCode,ShipCountry")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orders).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CompanyName", orders.CustomerID);
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName", orders.EmployeeID);
            ViewBag.ShipVia = new SelectList(db.Shippers, "ShipperID", "CompanyName", orders.ShipVia);
            return View(orders);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Orders orders = db.Orders.Find(id);
            var order_Details = db.Order_Details.Include(o => o.Orders).Include(o => o.Products).Where(p => p.OrderID == id).ToList();
            if (!order_Details.Any())
            {
                db.Orders.Remove(orders);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            Session["ErrorMsgDeleteCascada"] = "Primero elimine los detalles de la orden";
            return RedirectToAction("Delete", "Orders", new { id = orders.OrderID }); ;

        }

        [HttpGet]
        public FileContentResult ExportToExcel()
        {
            List<Orders> ordenesExcel = db.Orders.OrderByDescending(p => p.OrderID).ToList();
            string[] columns = { "OrderID", "CustomerID", "EmployeeID", "OrderDate", "RequiredDate", "ShippedDate", "ShipVia", "Freight", "ShipName", "ShipAddress", "ShipCity", "ShipRegion", "ShipPostalCode", "ShipCountry" };
            byte[] filecontent = ExcelExportUtil.ExportExcel(ordenesExcel, "Ordenes", true, columns);
            return File(filecontent, ExcelExportUtil.ExcelContentType, "Ordenes.xlsx");
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
