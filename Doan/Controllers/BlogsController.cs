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
using PagedList;

namespace Doan.Controllers
{
    public class BlogsController : Controller
    {
        private Db_Doan db = new Db_Doan();

        // GET: Blogs
        public ActionResult Index()
        {
            return View(db.Blogs.ToList());
        }
        public ActionResult IndexUser(int? page)
        {
            return View(db.Blogs.ToList().ToPagedList(page ?? 1, 9));
        }
        //public ViewHistory GetViewHistory()
        //{
        //    ViewHistory vh = Session["ViewHistory"] as ViewHistory;
        //    if (vh == null || Session["ViewHistory"] == null)
        //    {
        //        vh = new ViewHistory();
        //        Session["ViewHistory"] = vh;
        //    }
        //    return vh;
        //}
        public ActionResult BlogDetails(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            var bg = db.Blogs.SingleOrDefault(s => s.Id == id);
            //if (bg != null)
            //{
            //    GetViewHistory().Add(pro);
            //}
            return View(blog);
        }

        // GET: Blogs/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // GET: Blogs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Image,MoreImage,Content,Title")] Blog blog, HttpPostedFileBase Image, HttpPostedFileBase MoreImage)

        {
            if (ModelState.IsValid)
            {
                if (Image != null && Image.ContentLength > 0)
                    try
                    {
                        var fileName = Path.GetFileName(Image.FileName);
                        blog.Image = fileName;
                        var path = Path.Combine(Server.MapPath("~/Images"), fileName);
                        Image.SaveAs(path);
                    }
                    catch
                    {

                    }
                if (MoreImage != null && MoreImage.ContentLength > 0)
                    try
                    {
                        var fileName1 = Path.GetFileName(MoreImage.FileName);
                        blog.MoreImage = fileName1;
                        var path1 = Path.Combine(Server.MapPath("~/Images"), fileName1);
                        Image.SaveAs(path1);
                    }
                    catch
                    {

                    }
                db.Blogs.Add(blog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blog);
        } 
          

        // GET: Blogs/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Image,MoreImage,Content,Title")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(blog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blog);
        }

        // GET: Blogs/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Blog blog = db.Blogs.Find(id);
            db.Blogs.Remove(blog);
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
