using Doan.Models;
using Doan.Models.Common;
using Doan.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Doan.Controllers
{
    public class OrderManagementController : Controller
    {
        // GET: OrderManagement
        Db_Doan db = new Db_Doan();
        public ActionResult Index()
        {
            return View();
        }

        //chua thanh toan
        public ActionResult Unpaid()
        {
            var result = from r in db.Orders
                         where r.Status == false && r.Deleted == false
                         select new OrderViewModel()
                         {
                             IDOrder = r.IDOrder,
                             CodeCus = r.CodeCus,
                             Status = r.Status,
                             OrderDate = r.OrderDate,
                         };
            var ls = result.OrderBy(n => n.OrderDate);
            return View(ls);
        }

        //duyet don hang
        [HttpGet]
        public ActionResult OrderApproval(int? id)
        {
            ViewBag.ID_Ship = new SelectList(db.Shipping_Units, "ID_Ship", "Name");
            ViewBag.unitShip = new SelectList(getSup(), "ID_Ship", "Name");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            _Order model = db.Orders.SingleOrDefault(n => n.IDOrder == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            var lstChiTietDH = db.OrderDetails.Where(n => n.IDOrder == id);
            ViewBag.ListChiTietDH = lstChiTietDH;
            return View(model);
        }

        [HttpGet]
        public ActionResult OdDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            _Order model = db.Orders.SingleOrDefault(n => n.IDOrder == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            var lstChiTietDH = db.OrderDetails.Where(n => n.IDOrder == id);
            ViewBag.ListChiTietDH = lstChiTietDH;
            return View(model);
        }

        /// <param name="ctdh"></param>
        [HttpPost]
        public ActionResult OdDetails(_Order ctdh)
        {
            _Order ddhUpdate = db.Orders.Where(n => n.IDOrder.Equals(ctdh.IDOrder)).FirstOrDefault();
            if (ddhUpdate != null)
            {
                var lstChiTietDH = db.OrderDetails.Where(n => n.IDOrder == ctdh.IDOrder);
                ViewBag.ListChiTietDH = lstChiTietDH;
                return View(ddhUpdate);
            }
            else
            {
                ViewBag.Message = "Chon hoa don";
            }
            return View(ddhUpdate);
        }
        /// <param name="ddh"></param>
        [HttpPost]
        public ActionResult OrderApproval(_Order ddh)
        {
            ViewBag.ID_Ship = new SelectList(db.Shipping_Units, "ID_Ship", "Name");
            ViewBag.unitShip = new SelectList(getSup(), "ID_Ship", "Name");
            _Order ddhUpdate = db.Orders.Where(n => n.IDOrder.Equals(ddh.IDOrder)).FirstOrDefault();
            if (ddhUpdate != null)
            {
                if (ddh.Status == true && ddh.Paid == false)
                {
                    ddhUpdate.Delivery_date = DateTime.Now;
                }
                ddhUpdate.Paid = ddh.Paid;
                ddhUpdate.Status = true;
                ddhUpdate.Delivery_date = DateTime.Now;
                ddhUpdate.ID_Ship = ddh.ID_Ship;
                ddhUpdate.TotalSuccess = ddhUpdate.TotalSuccess + ddh.Shipping.Shipping_fee;
                db.SaveChanges();
                var lstChiTietDH = db.OrderDetails.Where(n => n.IDOrder == ddh.IDOrder);
                ViewBag.ListChiTietDH = lstChiTietDH;
                ViewBag.Message = "Lưu thành công";
            }
            else
            {
                ViewBag.Message = "Lưu không thành công";
            }
            return View(ddhUpdate);
        }

        //chua giao
        public ActionResult Delivery()
        {
            var c = from a in db.Orders
                    where a.Status == true && a.Paid == false && a.Deleted == false
                    select new OrderViewModel()
                    {
                        IDOrder = a.IDOrder,
                        Email_Cus = a.Customer.Email_Cus,
                        CodeCus = a.CodeCus,
                        Status = a.Status,
                        OrderDate = a.OrderDate,
                    };
            var lst = c.OrderBy(n => n.OrderDate);
            return View(lst);
        }

        public ActionResult UpdateOrder(int? id)
        {
            ViewBag.ID_Ship = new SelectList(db.Shipping_Units, "ID_Ship", "Name");
            ViewBag.unitShip = new SelectList(getSup(), "ID_Ship", "Name");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            _Order model = db.Orders.SingleOrDefault(n => n.IDOrder == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            var lstChiTietDH = db.OrderDetails.Where(n => n.IDOrder == id);
            ViewBag.ListChiTietDH = lstChiTietDH;
            return View(model);
        }
        public List<Shipping_unit> getSup()
        {
            Db_Doan db = new Db_Doan();
            List<Shipping_unit> prolst = db.Shipping_Units.ToList();
            return prolst;
        }

        /// <param name="cndh"></param>
        [HttpPost]
        public ActionResult UpdateOrder(_Order cndh)
        {
           
            _Order ddhUpdate = db.Orders.Where(n => n.IDOrder.Equals(cndh.IDOrder)).FirstOrDefault();
            if (ddhUpdate != null)
            {
                if (cndh.Status == true && cndh.Paid == false)
                {
                    ddhUpdate.Delivery_date = DateTime.Now;
                }
                ddhUpdate.Paid = cndh.Paid;
                ddhUpdate.Status = cndh.Status;
                ddhUpdate.ID_Ship = cndh.ID_Ship;
                ddhUpdate.TotalSuccess = ddhUpdate.TotalSuccess + cndh.Shipping.Shipping_fee;
                ddhUpdate.Delivery_date = DateTime.Now;
                db.SaveChanges();
                var lstChiTietDH = db.OrderDetails.Where(n => n.IDOrder == cndh.IDOrder);
                ViewBag.ListChiTietDH = lstChiTietDH;
                ViewBag.Message = "Lưu thành công";
                return RedirectToAction("Delivery", "OrderManagement");
            }
            else
            {
                ViewBag.Message = "Lưu không thành công";
            }
            ViewBag.ID_Ship = new SelectList(db.Shipping_Units, "ID_Ship", "Name", ddhUpdate.ID_Ship);
            return View(ddhUpdate);

        }
        //da giao, da thanh toan
        public ActionResult Success()
        {
            var result = from r in db.Orders
                         where r.Status == true && r.Paid == true && r.Deleted==false
                         select new OrderViewModel()
                         {
                             IDOrder = r.IDOrder,
                             CodeCus = r.CodeCus,
                             Status = r.Status,
                             OrderDate = r.OrderDate,
                         };
            var ls = result.OrderBy(n => n.OrderDate);
            return View(ls);
        }

        // danh sach don da huy
        public ActionResult Deleted()
        {
            var result = from r in db.Orders
                         where r.Deleted == true
                         select new OrderViewModel()
                         {
                             IDOrder = r.IDOrder,
                             CodeCus = r.CodeCus,
                             Status = r.Status,
                             OrderDate = r.OrderDate,
                         };
            var ls = result.OrderBy(n => n.OrderDate);
            return View(ls);
        }
        public ActionResult AllBill()
        {
            return View(db.Orders.ToList());
        }


        public ActionResult DeletedBill(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            _Order model = db.Orders.SingleOrDefault(n => n.IDOrder == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            var lstChiTietDH = db.OrderDetails.Where(n => n.IDOrder == id);
            ViewBag.ListChiTietDH = lstChiTietDH;
            return View(model);
        }

        /// <param name="hdh"></param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletedBill(_Order hdh)
        {
            _Order ddhUpdate = db.Orders.Where(n => n.IDOrder.Equals(hdh.IDOrder)).FirstOrDefault();


            if (ddhUpdate != null)
            {
                var lstorderDetail = db.OrderDetails.Where(n => n.IDOrder == ddhUpdate.IDOrder);

                foreach (var item in lstorderDetail)
                {
                    Product _pro = db.Products.Find(item.IDProduct);
                    _pro.Quantity = _pro.Quantity + item.QuantitySale;
                }
                if (hdh.Status == true && hdh.Paid == false)
                {
                    ddhUpdate.Delivery_date = DateTime.Now;
                }

                ddhUpdate.Deleted = true;
                ddhUpdate.DeletedDay = DateTime.Now;

                var sumprice = from sp in db.OrderDetails.AsEnumerable()
                               where sp.IDOrder == hdh.IDOrder
                               group sp by sp.IDOrder into g
                               select new
                               {
                                   ids = g.First().IDOrder,
                                   sums = g.Sum(x => x.UnitPriceSale)
                               };
                decimal total = sumprice.FirstOrDefault().sums.Value;

                ViewBag.Message = "Hủy thành công";
                db.SaveChanges();
                return RedirectToAction("Delivery", "OrderManagement");
            }
            else
            {
                ViewBag.Message = "Huy không thành công";
            }
            return View(ddhUpdate);
        }
    }
}