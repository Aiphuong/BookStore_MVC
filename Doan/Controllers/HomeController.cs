using Doan.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using Doan.Models.MD;

namespace Doan.Controllers
{
    public class HomeController : Controller
    {
        Db_Doan db = new Db_Doan();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexUser(string searchString1)
        {
            if (!string.IsNullOrEmpty(searchString1))
            {
                foreach(var i in db.Products)
                {
                    if (i.ProductName == searchString1)
                    {
                        return RedirectToAction("ProductDetail", "Home", new { id = i.IDProduct });
                    }
                }
                
            }

            return View(db.Products.Where(s => s.Quantity > 0).ToList());
        }

        public ActionResult Menu()
        {
            return View(db.Categories.ToList());
        }

        public ActionResult slide()
        {
            return View(db.Slides.ToList());
        }

        public ActionResult NewProducts()
        {
            return View(db.Products.Where(s => s.Quantity > 0).OrderBy(m=>m.CreatedDate).Take(10).ToList());
        }

        public ActionResult bestbook()
        {
            var result = from r in db.Products
                         orderby r.Quantity descending
                         select r;
            return View(result.Where(s => s.Quantity > 0).ToList()); 
        }

        public ActionResult BestSale()
        {
            var bs = new BestSaleMD();
            return View(bs.GetBestSale().OrderByDescending(m => m.Quantity));
        }

        public ActionResult CategoryList()
        {
            return View(db.Categories.ToList());
        }
        public ActionResult SuppliersList()
        {
            return View(db.Suppliers.ToList());
        }

        public ViewHistory GetViewHistory()
        {
            ViewHistory vh = Session["ViewHistory"] as ViewHistory;
            if (vh == null || Session["ViewHistory"] == null)
            {
                vh = new ViewHistory();
                Session["ViewHistory"] = vh;
            }
            return vh;
        }

        public ActionResult ProductDetail(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            var pro = db.Products.SingleOrDefault(s => s.IDProduct == id);
            if (pro != null)
            {
                GetViewHistory().Add(pro);
            }
            return View(product);
        }

        public ActionResult ShowToGetViewHistoryt()
        {
            ViewHistory vh = Session["ViewHistory"] as ViewHistory;
            return View(vh);
        }

        public ActionResult MoreProduct(long? id)
        {
            var result = from r in db.Products
                         where r.CategoryID == id
                         select r;
            return PartialView("MoreProduct", result.Where(s => s.Quantity > 0));
        }

        public ActionResult ProductList(long? id, int ? page)
        {
            ViewBag.banner = db.Slides.FirstOrDefault();
            var result = from r in db.Products
                         where r.CategoryID == id
                         select r;
            ViewBag.GiaGiam = result.ToList().OrderBy(n => n.Price);
            return PartialView("ProductList", result.Where(s => s.Quantity > 0).ToList().ToPagedList(page??1,9));

        }

        public ActionResult ProductListWithSupplier(long? id, int? page)
        {
            var result = from r in db.Products
                         where r.SupplierID == id
                         select r;
            return PartialView("ProductListWithSupplier", result.Where(s => s.Quantity > 0).ToList().ToPagedList(page ?? 1, 9));
        }
        public ActionResult SanPhamGiaGiamDan(long? id, int? page)
        {
            var result = from r in db.Products
                         where r.CategoryID == id
                         select r;
            //ViewBag.GiaTang = result.ToList().OrderBy(n => n.Price);
            //ViewBag.GiaGiam = result.ToList().OrderByDescending(n => n.Price);
            return PartialView("SanPhamGiaGiamDan", result.Where(s => s.Quantity > 0).ToList().OrderBy(n =>n.Price).ToPagedList(page ?? 1, 9));
        }

       
        public ActionResult SanPhamGiaTangDan(long? id, int? page)
        {
            var result = from r in db.Products
                         where r.CategoryID == id
                         select r;
            return PartialView("SanPhamGiaTangDan", result.Where(s => s.Quantity > 0).ToList().OrderByDescending(n => n.Price).ToPagedList(page ?? 1, 9));
        }

        public ActionResult SanPhamGiaTangDanTheoNhaCC(long? id, int? page)
        {
            var result = from r in db.Products
                         where r.SupplierID == id
                         select r;
            return PartialView("SanPhamGiaTangDanTheoNhaCC", result.Where(s => s.Quantity > 0).ToList().OrderByDescending(n => n.Price).ToPagedList(page ?? 1, 9));
        }

        public ActionResult SanPhamGiaGiamDanTheoNhaCC(long? id, int? page)
        {
            var result = from r in db.Products
                         where r.SupplierID == id
                         select r;
            //ViewBag.GiaTang = result.ToList().OrderBy(n => n.Price);
            //ViewBag.GiaGiam = result.ToList().OrderByDescending(n => n.Price);
            return PartialView("SanPhamGiaGiamDanTheoNhaCC", result.Where(s => s.Quantity > 0).ToList().OrderBy(n => n.Price).ToPagedList(page ?? 1, 9));
        }

    }
}