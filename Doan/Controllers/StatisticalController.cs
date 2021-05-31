using Doan.Models;
using Doan.Models.MD;
using Doan.Models.ViewModel;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Doan.Controllers
{
    public class StatisticalController : BaseController
    {
        // GET: Statistical
        Db_Doan db = new Db_Doan();
        public ActionResult Index()
        {
            ViewBag.ThongKeDoanhThu = ThongKeDoanhThu();
            ViewBag.ThongKeDonHang = ThongKeDonHang(); 
            ViewBag.ThongKeLoaiSanPham = ThongKeLoaiSanPham();
            ViewBag.ThongKeKhachHang = ThongKeKhachHang();
            ViewBag.ThongKeDoanhThuSau = ThongKeDoanhThuSau();
            ViewBag.ThongKeSoTienNhanDuoc = ThongKeSoTienNhanDuoc();
            ViewBag.ThongKeSoTienCongNo = ThongKeSoTienCongNo();
            return View();
        }
        //danh sach san pham ban chay
        public ActionResult BestSale()
        {
            var bs = new BestSaleMD();
            return View(bs.GetBestSale().OrderByDescending(m=>m.Quantity));
        }
//        var sumprice = from sp in db.OrderDetails.AsEnumerable() //Trả về đầu vào được gõ là IEnumerable <T> .
//                       group sp by sp.IDOrder into g
//                       select new
//                       {
//                           ids = g.First().IDOrder,
//                           sums = g.Sum(x => x.UnitPriceSale)
//                       };

//        var max = sumprice.OrderByDescending(m => m.sums).Take(1);
//        List<_Order> h = new List<_Order>();
//            foreach (var item in db.Orders)
//            {
//                if (item.IDOrder == max.First().ids)
//                {
//                    h.Add(item);
//                }
//}

//            return View(h.ToList());

        //danh sachs don con cong no
        public ActionResult DonCongNo()
        {
            var result = from bill in db.Orders
                         where bill.Collected < bill.TotalSuccess
                         select bill;
            return View(result.Where(m=>m.Status == true).ToList().OrderBy(n => n.OrderDate));
        }
        //danh sach don hang da thu tien
        public ActionResult DonDaThu()
        {
            var result = from bill in db.Orders
                         where bill.Collected == bill.TotalSuccess
                         select bill;
            return View(result.Where(m => m.Status == true && m.Paid == true).ToList().OrderBy(n => n.OrderDate));
        }

        //danh sach don hang da huy
        public ActionResult DonDaHuy()
        {
            return View(db.Orders.Where(m => m.Deleted == true).ToList().OrderBy(n => n.OrderDate));
        }

        //danh sach san pham sắp hết
        public ActionResult spsaphet()
        {
            List<Product> h = new List<Product>();
            foreach (var item in db.Products)
            {
                if (item.Quantity>0 && item.Quantity<=5)
                {
                    h.Add(item);
                }
            }
            return View(h.OrderByDescending(m=>m.Quantity).ToList());
        }
        public ActionResult DonTheoDonViVanChuyen(long? id)
        {
            var result = from bill in db.Orders
                         where bill.ID_Ship == id
                         select bill;
            return View(result.Where(m => m.Status == true).ToList().OrderBy(n => n.OrderDate));
        }
        //danh sach san pham đã hết hàng
        public ActionResult sphethang()
        {
            List<Product> h = new List<Product>();
            foreach (var item in db.Products)
            {
                if (item.Quantity <= 0)
                {
                    h.Add(item);
                }
            }
            return View(h.OrderByDescending(m => m.Quantity).ToList());
        }

        //tong doanh thu du kien
        public decimal ThongKeDoanhThu()
        {
            decimal TongDoanhThu = db.OrderDetails.Sum(n => n.QuantitySale * n.UnitPriceSale).Value;
            return TongDoanhThu;
        }

        //doanh thu ban thuc te (sau khi tru coupon+ the thanh vien)
        public decimal ThongKeDoanhThuSau()
        {
            decimal TongDoanhThu = db.Orders.Sum(n => n.TotalSuccess);
            return TongDoanhThu;
        }
        //thong ke so tien da nhan duoc
        public decimal ThongKeSoTienNhanDuoc()
        {
            decimal TongDoanhThu1 = db.Orders.Sum(n => n.Collected);
            return TongDoanhThu1;
        }

        //thong ke don giao khong thanh cong (ngay giao voi ngay nhan qua 10 ngay)
        public decimal ThongKeDonGiaoKhongThanhCong()
        {
            decimal TongDoanhThu2 = db.Orders.Sum(n => n.Collected);
            return TongDoanhThu2;
        }

        //Thong ke so tien con cong no
        public decimal ThongKeSoTienCongNo()
        {
            decimal TongDoanhThu3 = db.Orders.Sum(n => n.TotalSuccess - n.Collected);
            return TongDoanhThu3;
        }
        //tong so khach hang
        public double ThongKeKhachHang()
        {
            double slkh = db.Customers.Count();
            return slkh;
        }
        //tong so luong don hang
        public double ThongKeDonHang()
        {
            double slddh = db.Orders.Count();
            return slddh;
        }
        //thong ke loai san pham
        public double ThongKeLoaiSanPham()
        {
            double slncc = db.Categories.Count();
            return slncc;
        }



        //xem hoa don co gia tri lon nhat
        public ActionResult MaxPrice()
        {
            var sumprice = from sp in db.OrderDetails.AsEnumerable() //Trả về đầu vào được gõ là IEnumerable <T> .
                           group sp by sp.IDOrder into g
                           select new
                           {
                               ids = g.First().IDOrder,
                               sums = g.Sum(x => x.UnitPriceSale)
                           };

            var max = sumprice.OrderByDescending(m => m.sums).Take(1);
            List<_Order> h = new List<_Order>();
            foreach (var item in db.Orders)
            {
                if (item.IDOrder == max.First().ids)
                {
                    h.Add(item);
                }
            }

            return View(h.ToList());
        }
        
        //hoa don ban duoc theo ngay
        public ActionResult doanhthutheongay(DateTime? fromDate, DateTime? toDate)
        {
            var timkiem = from hd in db.Orders
                          join cthd in db.OrderDetails on hd.IDOrder equals cthd.IDOrder into t
                          from cthd in t.DefaultIfEmpty()
                          select new hoadontheongay
                          {
                              OrderDate = hd.OrderDate,
                              IDOrder = cthd.IDOrder,
                              CodeCus = hd.CodeCus,
                              QuantitySale = t.Sum(s => s.QuantitySale),
                              UnitPriceSale = t.Sum(s => s.UnitPriceSale)

                          };
            var hoadon = timkiem.OrderByDescending(m => m.OrderDate);
            if (!fromDate.HasValue) fromDate = DateTime.Now.Date;
            if (!toDate.HasValue) toDate = fromDate.GetValueOrDefault(DateTime.Now.Date).Date.AddDays(1);
            if (toDate <= fromDate) toDate = fromDate.GetValueOrDefault(DateTime.Now.Date).Date.AddDays(1);
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            var search = hoadon.Where(c => c.OrderDate >= fromDate && c.OrderDate <= toDate).ToList();
            return View(search);
        }
        //doanh thu theo ngay
        public ActionResult hoadontheongay(DateTime? fromDate)
        {
            var timkiem = from hd in db.Orders
                          join cthd in db.OrderDetails on hd.IDOrder equals cthd.IDOrder into t
                          from cthd in t.DefaultIfEmpty()//Trả về các phần tử của một bộ sưu tập <T> IEnountable hoặc bộ sưu tập singleton có giá trị mặc định nếu chuỗi trống.
                          select new hoadontheongay
                          {
                              OrderDate = hd.OrderDate,
                              IDOrder = cthd.IDOrder,
                              CodeCus = hd.CodeCus,
                              QuantitySale = t.Sum(s => s.QuantitySale),
                              UnitPriceSale = t.Sum(s => s.UnitPriceSale)
                          };
            if (!fromDate.HasValue) fromDate = DateTime.Now.Date;
            ViewBag.fromDate = fromDate;

            List<hoadontheongay> h = new List<hoadontheongay>();
            foreach (var item in timkiem)
            {
                if (item.OrderDate.Value.Date == fromDate.Value.Date)
                {
                    h.Add(item);
                }
            }
            var result = from r in h
                         group r by r.OrderDate into g
                         select new doanhthutheongay
                         {
                             OrderDate = g.First().OrderDate,
                             QuantityOrder = g.Count(),
                             Total = g.Sum(s => s.UnitPriceSale)
                         };
            var search = result.Where(c => c.OrderDate >= fromDate).ToList();
            var allDays = MoreEnumerable.GenerateByIndex(i => fromDate.Value.AddDays(i)).Take(30); //Trả về một chuỗi các giá trị dựa trên các chỉ mục.

            var salesByDay = from s in search
                             group s by s.OrderDate.Value.Date into g
                             select new { Day = g.Key, 
                                          totalSales = g.Sum(x => (decimal)x.Total),
                                          totalquantity = g.Sum(x=>x.QuantityOrder)};

            var query = from s in salesByDay
                        join d in allDays on s.Day equals d
                        select new doanhthutheongay
                        {
                            OrderDate = s.Day,
                            QuantityOrder=(s!= null)? s.totalquantity : 0,
                            Total = (s != null) ? s.totalSales : 0m };
            return View(query);
        }

        public ActionResult GetData()
        {
            var query = db.OrderDetails.Include("Product")
                  .GroupBy(p => p.Product.ProductName)
                  .Select(g => new { name = g.Key, count = g.Sum(w => w.QuantitySale) }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetData2()
        {
            var query = db.Products.Include("Category")
                  .GroupBy(p => p.Category.CategoryName)
                  .Select(g => new { name = g.Key, count = g.Count() });
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetData3()
        {
            var result = from bs in db.OrderDetails.AsEnumerable()
                           group bs by bs.IDProduct into g
                           orderby g.Sum(x => x.IDProduct) descending
                           select new 
                           {
                               name = g.First().Product.ProductName,
                               count = g.Sum(x => x.QuantitySale)
                           };
            var temp= result.Take(9).ToList();
            var query = temp.OrderByDescending(m => m.count).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);


        }
        public ActionResult GetData4()
        {

            var result = from g in db.Products
                        where g.Quantity > 0 && g.Quantity <= 5
                        select new {
                            name = g.ProductName,
                            count = g.Quantity
                        };
            var query = result.OrderByDescending(m => m.count).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
    }
}