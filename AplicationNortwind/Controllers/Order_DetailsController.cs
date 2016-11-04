using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AplicationNortwind.Models;

namespace AplicationNortwind.Controllers
{
    [Authorize]
    public class Order_DetailsController : Controller
    {
        private DataAccess db = new DataAccess();

        // GET: Order_Details/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order_Details order_Details = db.Order_Details.Find(id);
            if (order_Details == null)
            {
                return RedirectToAction("Index", "Orders");
            }
            return View(order_Details);
        }

        // GET: Order_Details/Create
        public ActionResult Create(int? idOrder)
        {
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderID", idOrder);
            var order_Details = db.Order_Details.Where(p => p.OrderID == idOrder).ToList();
            //var productos = db.Products.Where(p => !order_Details.Any(p2 => p2.ProductID = p.ProductID));
            //ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName").Where(p => !order_Details.Any(p2 => p2.ProductID = p.Value));
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
            return View();
        }

        // POST: Order_Details/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,ProductID,UnitPrice,Quantity,Discount")] Order_Details order_Details)
        {
            if (ModelState.IsValid)
            {
                Order_Details orderTemp = db.Order_Details.Find(order_Details.OrderID, order_Details.ProductID);
                if (orderTemp == null)
                {
                    db.Order_Details.Add(order_Details);
                    db.SaveChanges();
                    return RedirectToAction("Details", "Orders", new { id = order_Details.OrderID });
                }
                ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderID", order_Details.OrderID);
                ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
                ModelState.AddModelError("", "La Orden ya existe");
                return View(order_Details);
            }

            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "CustomerID", order_Details.OrderID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", order_Details.ProductID);
            return View(order_Details);
        }

        // GET: Order_Details/Edit/5
        public ActionResult Edit(int? id, int? productId)
        {
            if (id == null || productId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order_Details order_Details = db.Order_Details.Find(id, productId);
            if (order_Details == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "CustomerID", order_Details.OrderID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", order_Details.ProductID);
            return View(order_Details);
        }

        // POST: Order_Details/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,ProductID,UnitPrice,Quantity,Discount")] Order_Details order_Details)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order_Details).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Orders", new { id = order_Details.OrderID });
            }
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "CustomerID", order_Details.OrderID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", order_Details.ProductID);
            return View(order_Details);
        }

        // GET: Order_Details/Delete/5
        public ActionResult Delete(int? id, int? productId)
        {
            if (id == null || productId==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order_Details order_Details = db.Order_Details.Find(id, productId);
            if (order_Details == null)
            {
                return HttpNotFound();
            }
            return View(order_Details);
        }

        // POST: Order_Details/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int productid)
        {
            Order_Details order_Details = db.Order_Details.Find(id, productid);
            db.Order_Details.Remove(order_Details);
            db.SaveChanges();
            return RedirectToAction("Details", "Orders", new {id = id });
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
