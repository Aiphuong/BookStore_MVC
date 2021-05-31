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
    public class Shipping_unitController : Controller
    {
        private Db_Doan db = new Db_Doan();

        // GET: Shipping_unit
        public ActionResult Index()
        {
            return View(db.Shipping_Units.ToList());
        }

        // GET: Shipping_unit/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipping_unit shipping_unit = db.Shipping_Units.Find(id);
            if (shipping_unit == null)
            {
                return HttpNotFound();
            }
            return View(shipping_unit);
        }

        // GET: Shipping_unit/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Shipping_unit/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Ship,Name")] Shipping_unit shipping_unit)
        {
            if (ModelState.IsValid)
            {
                db.Shipping_Units.Add(shipping_unit);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(shipping_unit);
        }

        // GET: Shipping_unit/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipping_unit shipping_unit = db.Shipping_Units.Find(id);
            if (shipping_unit == null)
            {
                return HttpNotFound();
            }
            return View(shipping_unit);
        }

        // POST: Shipping_unit/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Ship,Name")] Shipping_unit shipping_unit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shipping_unit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(shipping_unit);
        }

        // GET: Shipping_unit/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipping_unit shipping_unit = db.Shipping_Units.Find(id);
            if (shipping_unit == null)
            {
                return HttpNotFound();
            }
            return View(shipping_unit);
        }

        // POST: Shipping_unit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Shipping_unit shipping_unit = db.Shipping_Units.Find(id);
            db.Shipping_Units.Remove(shipping_unit);
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
