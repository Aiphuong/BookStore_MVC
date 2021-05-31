using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Doan.Models;

namespace Doan.Controllers
{
    public class ShippingsController : Controller
    {
        private Db_Doan db = new Db_Doan();

        // GET: Shippings
        public ActionResult Index()
        {
            var shippings = db.Shippings.Include(s => s.Shipping_Unit);
            return View(shippings.ToList());
        }

        // GET: Shippings/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipping shipping = db.Shippings.Find(id);
            if (shipping == null)
            {
                return HttpNotFound();
            }
            return View(shipping);
        }

        // GET: Shippings/Create
        public ActionResult Create()
        {
            ViewBag.ID_Ship = new SelectList(db.Shipping_Units, "ID_Ship", "Name");
            return View();
        }

        // POST: Shippings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDShip,Total,Status,Shipping_fee,ID_Ship")] Shipping shipping)
        {
            if (ModelState.IsValid)
            {
                db.Shippings.Add(shipping);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_Ship = new SelectList(db.Shipping_Units, "ID_Ship", "Name", shipping.ID_Ship);
            return View(shipping);
        }

        // GET: Shippings/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipping shipping = db.Shippings.Find(id);
            if (shipping == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_Ship = new SelectList(db.Shipping_Units, "ID_Ship", "Name", shipping.ID_Ship);
            return View(shipping);
        }

        // POST: Shippings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDShip,Total,Status,Shipping_fee,ID_Ship")] Shipping shipping)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shipping).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_Ship = new SelectList(db.Shipping_Units, "ID_Ship", "Name", shipping.ID_Ship);
            return View(shipping);
        }

        // GET: Shippings/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipping shipping = db.Shippings.Find(id);
            if (shipping == null)
            {
                return HttpNotFound();
            }
            return View(shipping);
        }

        // POST: Shippings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Shipping shipping = db.Shippings.Find(id);
            db.Shippings.Remove(shipping);
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
