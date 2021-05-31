using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Doan.Models;
using Doan.Models.Common;

namespace Doan.Controllers
{
    public class ProductsController : BaseController
    {
        private Db_Doan db = new Db_Doan();

        // GET: Products
        //[Authorize]
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category).Include(p => p.Supplier);
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "SupplierName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Create([Bind(Include = "IDProduct,ProductName,MetaTitle,Description,Image,MoreImage1,MoreImage2,MoreImage3,Price,Entryprice,PromotionPrice,IncludedVAT,Quantity,CategoryID,SupplierID,Detail,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy")] Product product, HttpPostedFileBase Image, HttpPostedFileBase MoreImage1, HttpPostedFileBase MoreImage2, HttpPostedFileBase MoreImage3)
        {
            var session = (UserLogin)Session[CommonConstants.USER_SESSION];

            if (ModelState.IsValid)
            {
               
                if (Image != null && Image.ContentLength > 0)
                    try
                    {
                        var fileName = Path.GetFileName(Image.FileName);
                        product.Image = fileName;
                        var path = Path.Combine(Server.MapPath("~/Images"), fileName);
                        Image.SaveAs(path);
                    }
                    catch
                    {

                    }
                    if ( MoreImage1 != null && MoreImage1.ContentLength > 0 )
                    try
                    {
                        var fileName1 = Path.GetFileName(MoreImage1.FileName);
                        product.MoreImage1 = fileName1;
                        var path1 = Path.Combine(Server.MapPath("~/Images"), fileName1);
                        Image.SaveAs(path1);
                    }
                    catch
                    {

                    }
                    if ( MoreImage2 != null && MoreImage2.ContentLength > 0 )
                    try
                    {
                        var fileName2 = Path.GetFileName(MoreImage2.FileName);
                        product.MoreImage2 = fileName2;
                        var path2 = Path.Combine(Server.MapPath("~/Images"), fileName2);
                        Image.SaveAs(path2);
                    }
                    catch
                    {

                    }
                    if ( MoreImage3 != null && MoreImage3.ContentLength > 0)
                    try
                    {
                        var fileName3 = Path.GetFileName(MoreImage3.FileName);
                        product.MoreImage3 = fileName3;
                        var path3 = Path.Combine(Server.MapPath("~/Images"), fileName3);
                        Image.SaveAs(path3);
                    }
                    catch
                    {

                    }
                product.CreatedDate = DateTime.Now;
                product.CreatedBy = session.UserName;
                db.Products.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "SupplierName", product.SupplierID);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "SupplierName", product.SupplierID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDProduct,ProductName,MetaTitle,Description,Image,MoreImage1,MoreImage2,MoreImage3,Price,Entryprice,PromotionPrice,IncludedVAT,Quantity,CategoryID,SupplierID,Detail,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy")] Product product, HttpPostedFileBase Image, HttpPostedFileBase MoreImage1, HttpPostedFileBase MoreImage2, HttpPostedFileBase MoreImage3)
        {
            if (ModelState.IsValid)
            {
                var session = (UserLogin)Session[CommonConstants.USER_SESSION];
                if (product.Image != null)
                    try
                    {
                        var fileName = Path.GetFileName(Image.FileName);
                        product.Image = fileName;
                        var path = Path.Combine(Server.MapPath("~/Images"), fileName);
                        Image.SaveAs(path);
                    }
                    catch
                    {

                    }
                if (MoreImage1 != null && MoreImage1.ContentLength > 0)
                    try
                    {
                        var fileName1 = Path.GetFileName(MoreImage1.FileName);
                        product.MoreImage1 = fileName1;
                        var path1 = Path.Combine(Server.MapPath("~/Images"), fileName1);
                        Image.SaveAs(path1);
                    }
                    catch
                    {

                    }
                if (MoreImage2 != null && MoreImage2.ContentLength > 0)
                    try
                    {
                        var fileName2 = Path.GetFileName(MoreImage2.FileName);
                        product.MoreImage2 = fileName2;
                        var path2 = Path.Combine(Server.MapPath("~/Images"), fileName2);
                        Image.SaveAs(path2);
                    }
                    catch
                    {

                    }
                if (MoreImage3 != null && MoreImage3.ContentLength > 0)
                    try
                    {
                        var fileName3 = Path.GetFileName(MoreImage3.FileName);
                        product.MoreImage3 = fileName3;
                        var path3 = Path.Combine(Server.MapPath("~/Images"), fileName3);
                        Image.SaveAs(path3);
                    }
                    catch
                    {

                    }
                product.CreatedDate = product.CreatedDate;
                product.CreatedBy = session.UserName;
                product.ModifiedDate = DateTime.Now;
                product.ModifiedBy = session.UserName;
                db.Entry(product).State = EntityState.Modified;
                if (Image == null)
                    db.Entry(product).Property(m => m.Image).IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "SupplierName", product.SupplierID);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
