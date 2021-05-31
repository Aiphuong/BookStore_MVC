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
    public class IVCsController : Controller
    {
        private Db_Doan db = new Db_Doan();

        // GET: IVCs
        public ActionResult Index()
        {
            var iVCs = db.IVCs.Include(i => i.Supplier);
            return View(iVCs.ToList());
        }

        // GET: IVCs/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IVC iVC = db.IVCs.Find(id);
            if (iVC == null)
            {
                return HttpNotFound();
            }
            return View(iVC);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.MaNCC = new SelectList(getSup(), "SupplierID", "SupplierName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdSup,Date,Status,Supplier_SupplierID")] IVC iVC, IEnumerable<IVCDetail> lstModel)
        {
            iVC.Status = "false";
            iVC.Date = DateTime.Now;
            db.IVCs.Add(iVC);
            db.SaveChanges();
            Product sp;

            foreach (var item in lstModel)
            {
                sp = db.Products.Find(item.Id);
                sp.Quantity += item.Quantity;
                sp.PromotionPrice = item.Price;
                item.Ivc_Id = iVC.Id;
                item.Product_IDProduct = sp.IDProduct;
            }
            db.IVCDetails.AddRange(lstModel);
            db.SaveChanges();
            ViewBag.ThongBao = "Đã nhập hàng thành công";
            ViewBag.MaNCC = new SelectList(getSup(), "SupplierID", "SupplierName");
            return View();
        }



        public List<Supplier> getSup()
        {
            Db_Doan db = new Db_Doan();
            List<Supplier> prolst = db.Suppliers.ToList();
            return prolst;
        }

        public ActionResult getPro(long? supId)
        {
            Db_Doan db = new Db_Doan();

            List<Product> stateList = db.Products.Where(x => x.SupplierID == supId).ToList();

            ViewBag.StateOptions = new SelectList(stateList, "IDProduct", "ProductName");

            return PartialView("getPro");

        }



        // GET: IVCs/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IVC iVC = db.IVCs.Find(id);
            if (iVC == null)
            {
                return HttpNotFound();
            }
            ViewBag.Supplier_SupplierID = new SelectList(db.Suppliers, "SupplierID", "SupplierName", iVC.Supplier_SupplierID);
            return View(iVC);
        }

        // POST: IVCs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date,Status,Supplier_SupplierID")] IVC iVC)
        {
            if (ModelState.IsValid)
            {
                db.Entry(iVC).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Supplier_SupplierID = new SelectList(db.Suppliers, "SupplierID", "SupplierName", iVC.Supplier_SupplierID);
            return View(iVC);
        }

        // GET: IVCs/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IVC iVC = db.IVCs.Find(id);
            if (iVC == null)
            {
                return HttpNotFound();
            }
            return View(iVC);
        }

        // POST: IVCs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            IVC iVC = db.IVCs.Find(id);
            db.IVCs.Remove(iVC);
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
