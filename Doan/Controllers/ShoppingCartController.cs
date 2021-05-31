using Doan.Class;
using Doan.Models;
using Doan.Models.ClassLibrary;
using Doan.Models.Common;
using Doan.Models.Dao;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Payment = PayPal.Api.Payment;

namespace Doan.Controllers
{
    public class ShoppingCartController : Controller
    {
        // GET: ShoppingCart
        Db_Doan db = new Db_Doan();
        public Cart GetCart()
        {
            Cart cart = Session["Cart"] as Cart;
            if (cart == null || Session["Cart"] == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;
        }
        public ActionResult AddtoCart(long id)
        {
            var pro = db.Products.SingleOrDefault(s => s.IDProduct == id);
            if (pro != null)
            {
                GetCart().Add(pro);
            }
            return RedirectToAction("IndexUser", "Home", new { r = Request.Url.ToString() });
        }
        [HttpGet]
        public ActionResult Coupon()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Coupon(Coupon coupon)
        {
            var check = db.Coupons.Where(s => s.Name.Equals(coupon.Name)).ToList().AsEnumerable().FirstOrDefault();
            if (check == null)
            {
                ViewBag.ThongBao = "Ma Coupon khong kha dung";
                return RedirectToAction("Err", "ShoppingCart");
            }
            else
            {
                var result = from r in db.Coupons
                             where r.Name == coupon.Name
                             select r;
                var cus2 = result.ToList().First();
                var cpSession = new CouponSS();
                cpSession.Name = cus2.Name;
                cpSession.Price = cus2.Price;
                Session.Add(Models.Common.CommonCp.SESSION_NAME, cpSession);
                return RedirectToAction("ShowToCart", "ShoppingCart");
            }
        }
        public ActionResult ShowToCart()
        {

            if (Session["Cart"] == null)
            {
                return RedirectToAction("ShowToCart", "ShoppingCart");
            }
            Cart cart = Session["Cart"] as Cart;
            return View(cart);
        }
        public ActionResult Order_Success()
        {
            var session = (UserLogin)Session[Models.Common.CommonConstants.USER_SESSION];
            if (session == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("PurchaseAccount", "ShoppingCart");
            }
        }



        public ActionResult PurchaseAccount()
        {
            var session = (UserLogin)Session[Models.Common.CommonConstants.USER_SESSION];
            Customer cus = db.Customers.Find(session.UserID);
            return View(cus);
        }

        public ActionResult PurchaseAccountSuccess()
        {

            var session = (UserLogin)Session[Models.Common.CommonConstants.USER_SESSION];
            var sessioncp = (CouponSS)Session[Models.Common.CommonCp.SESSION_NAME];

            if (session == null)
            {
                return View();
            }
            else
            {
                if (sessioncp != null)
                {
                    string Tt = sessioncp.Name.ToString();
                    var res = (from r in db.Coupons
                               where (r.Name == Tt)
                               select r).AsEnumerable().FirstOrDefault();
                    Cart cart = Session["Cart"] as Cart;
                    var result = from r in db.Customers
                                 where r.CodeCus == session.UserID
                                 select r;
                    var cus2 = result.ToList().First();
                    _Order _order = new _Order();
                    _order.OrderDate = DateTime.Now;
                    _order.Email_Cus = cus2.Email_Cus;
                    _order.SDT_Cus = cus2.Phone_Cus;
                    _order.Password_cus = cus2.Password;
                    _order.Descriptions = cus2.Address_Cus;
                    _order.CodeCus = cus2.CodeCus;
                    _order.Deleted = false;
                    _order.Cancelled = false;
                    _order.Paid = false;
                    _order.Status = false;  db.Orders.Add(_order);

                    decimal total = 0;
                    foreach (var item in cart.Items)
                    {
                        OrderDetail _order_Detail = new OrderDetail();
                        _order_Detail.IDOrder = _order.IDOrder;
                        _order_Detail.IDProduct = item._shopping_product.IDProduct;
                        _order_Detail.UnitPriceSale = item._shopping_product.Price;
                        _order_Detail.QuantitySale = item._shopping_quantity;
                        _order_Detail.CreadeDay = DateTime.Now;
                        Product _pro = db.Products.Find(_order_Detail.IDProduct);
                        Customer customer1 = db.Customers.Find(cus2.CodeCus);
                        Customer customer = db.Customers.Where(n => n.IDCus.Equals(customer1.IDCus)).FirstOrDefault();
                        if (res != null)
                        {
                            if (_pro == null)
                            {
                                if (customer.Idtype != null)
                                {
                                    if (customer.Idtype == 3)
                                    {
                                        db.OrderDetails.Add(_order_Detail);
                                        total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price) / 10));
                                        _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                        _order.TotalSuccess = total;
                                        _order.IdCoupon = res.IdCoupon;
                                        _order.Collected = 0;
                                        _order.IdPayment = 1;
                                        if (total > 50)
                                        {
                                            if (customer != null)
                                            {
                                                decimal diem = total / 50;
                                                int diem2 = Convert.ToInt32(diem);
                                                customer.scores = customer.scores + diem2;
                                                if (customer.scores >= 50 && customer.scores <= 100)
                                                {
                                                    customer.Idtype = 3;
                                                    _order.scores = 3;
                                                }
                                                else if (customer.scores >= 100 && customer.scores <= 500)
                                                {
                                                    customer.Idtype = 4;
                                                    _order.scores = 4;
                                                }
                                                else if (customer.scores >= 500)
                                                {
                                                    customer.Idtype = 5;
                                                    _order.scores = 5;
                                                }
                                                db.Entry(customer1).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                    if (customer.Idtype == 4)
                                    {
                                        db.OrderDetails.Add(_order_Detail);
                                        total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price) / 15));
                                        _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                        _order.TotalSuccess = total;
                                        _order.IdCoupon = res.IdCoupon;
                                        _order.Collected = 0;
                                        _order.IdPayment = 1;
                                        if (total > 50)
                                        {
                                            if (customer != null)
                                            {
                                                decimal diem = total / 50;
                                                int diem2 = Convert.ToInt32(diem);
                                                customer.scores = customer.scores + diem2;
                                                if (customer.scores >= 50 && customer.scores <= 100)
                                                {
                                                    customer.Idtype = 3;
                                                    _order.scores = 3;
                                                }
                                                else if (customer.scores >= 100 && customer.scores <= 500)
                                                {
                                                    customer.Idtype = 4;
                                                    _order.scores = 4;
                                                }
                                                else if (customer.scores >= 500)
                                                {
                                                    customer.Idtype = 5;
                                                    _order.scores = 5;
                                                }
                                                db.Entry(customer1).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                    if (customer.Idtype == 5)
                                    {
                                        db.OrderDetails.Add(_order_Detail);
                                        total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price) / 20));
                                        _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                        _order.TotalSuccess = total;
                                        _order.IdCoupon = res.IdCoupon;
                                        _order.Collected = 0;
                                        _order.IdPayment = 1;
                                        if (total > 50)
                                        {
                                            if (customer != null)
                                            {
                                                decimal diem = total / 50;
                                                int diem2 = Convert.ToInt32(diem);
                                                customer.scores = customer.scores + diem2;
                                                if (customer.scores >= 50 && customer.scores <= 100)
                                                {
                                                    customer.Idtype = 3;
                                                    _order.scores = 3;
                                                }
                                                else if (customer.scores >= 100 && customer.scores <= 500)
                                                {
                                                    customer.Idtype = 4;
                                                    _order.scores = 4;
                                                }
                                                else if (customer.scores >= 500)
                                                {
                                                    customer.Idtype = 5;
                                                    _order.scores = 5;
                                                }
                                                db.Entry(customer1).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    db.OrderDetails.Add(_order_Detail);
                                    total += (_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price;
                                    _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                    _order.TotalSuccess = total;
                                    _order.IdCoupon = res.IdCoupon;
                                    _order.Collected = 0;
                                    _order.IdPayment = 1;
                                    if (total > 50)
                                    {
                                        if (customer != null)
                                        {
                                            decimal diem = total / 50;
                                            int diem2 = Convert.ToInt32(diem);
                                            customer.scores = customer.scores + diem2;
                                            if (customer.scores >= 50 && customer.scores <= 100)
                                            {
                                                customer.Idtype = 3;
                                                _order.scores = 3;
                                            }
                                            else if (customer.scores >= 100 && customer.scores <= 500)
                                            {
                                                customer.Idtype = 4;
                                                _order.scores = 4;
                                            }
                                            else if (customer.scores >= 500)
                                            {
                                                customer.Idtype = 5;
                                                _order.scores = 5;
                                            }
                                            db.Entry(customer1).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }

                            }
                            else
                            {
                                if (customer.Idtype != null)
                                {
                                    if (customer.Idtype == 3)
                                    {
                                        _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                        db.OrderDetails.Add(_order_Detail);
                                        total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price) / 10));
                                        _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                        _order.TotalSuccess = total;
                                        _order.IdCoupon = res.IdCoupon;
                                        _order.Collected = 0;
                                        _order.IdPayment = 1;
                                        if (total > 50)
                                        {
                                            if (customer != null)
                                            {
                                                decimal diem = total / 50;
                                                int diem2 = Convert.ToInt32(diem);
                                                customer.scores = customer.scores + diem2;
                                                if (customer.scores >= 50 && customer.scores <= 100)
                                                {
                                                    customer.Idtype = 3;
                                                    _order.scores = 3;
                                                }
                                                else if (customer.scores >= 100 && customer.scores <= 500)
                                                {
                                                    customer.Idtype = 4;
                                                    _order.scores = 4;
                                                }
                                                else if (customer.scores >= 500)
                                                {
                                                    customer.Idtype = 5;
                                                    _order.scores = 5;
                                                }
                                                db.Entry(customer1).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                    if (customer.Idtype == 4)
                                    {
                                        _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                        db.OrderDetails.Add(_order_Detail);
                                        total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price) / 15));
                                        _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                        _order.TotalSuccess = total;
                                        _order.IdCoupon = res.IdCoupon;
                                        _order.Collected = 0;
                                        _order.IdPayment = 1;
                                        if (total > 50)
                                        {
                                            if (customer != null)
                                            {
                                                decimal diem = total / 50;
                                                int diem2 = Convert.ToInt32(diem);
                                                customer.scores = customer.scores + diem2;
                                                if (customer.scores >= 50 && customer.scores <= 100)
                                                {
                                                    customer.Idtype = 3;
                                                    _order.scores = 3;
                                                }
                                                else if (customer.scores >= 100 && customer.scores <= 500)
                                                {
                                                    customer.Idtype = 4;
                                                    _order.scores = 4;
                                                }
                                                else if (customer.scores >= 500)
                                                {
                                                    customer.Idtype = 5;
                                                    _order.scores = 5;
                                                }
                                                db.Entry(customer1).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                    if (customer.Idtype == 5)
                                    {
                                        _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                        db.OrderDetails.Add(_order_Detail);
                                        total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price) / 20));
                                        _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                        _order.TotalSuccess = total;
                                        _order.IdCoupon = res.IdCoupon;
                                        _order.Collected = 0;
                                        _order.IdPayment = 1;
                                        if (total > 50)
                                        {
                                            if (customer != null)
                                            {
                                                decimal diem = total / 50;
                                                int diem2 = Convert.ToInt32(diem);
                                                customer.scores = customer.scores + diem2;
                                                if (customer.scores >= 50 && customer.scores <= 100)
                                                {
                                                    customer.Idtype = 3;
                                                    _order.scores = 3;
                                                }
                                                else if (customer.scores >= 100 && customer.scores <= 500)
                                                {
                                                    customer.Idtype = 4;
                                                    _order.scores = 4;
                                                }
                                                else if (customer.scores >= 500)
                                                {
                                                    customer.Idtype = 5;
                                                    _order.scores = 5;
                                                }
                                                db.Entry(customer1).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                    db.OrderDetails.Add(_order_Detail);
                                    total += (_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price;
                                    _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                    _order.TotalSuccess = total;
                                    _order.IdCoupon = res.IdCoupon;
                                    _order.Collected = 0;
                                    _order.IdPayment = 1;
                                    if (total > 50)
                                    {
                                        if (customer != null)
                                        {
                                            decimal diem = total / 50;
                                            int diem2 = Convert.ToInt32(diem);
                                            customer.scores = customer.scores + diem2;
                                            if (customer.scores >= 50 && customer.scores <= 100)
                                            {
                                                customer.Idtype = 3;
                                                _order.scores = 3;
                                            }
                                            else if (customer.scores >= 100 && customer.scores <= 500)
                                            {
                                                customer.Idtype = 4;
                                                _order.scores = 4;
                                            }
                                            else if (customer.scores >= 500)
                                            {
                                                customer.Idtype = 5;
                                                _order.scores = 5;
                                            }
                                            db.Entry(customer1).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (_pro == null)
                            {

                                if (customer.Idtype != null)
                                {
                                    if (customer.Idtype == 3)
                                    {
                                        db.OrderDetails.Add(_order_Detail);
                                        total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) / 10));
                                        _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                        _order.TotalSuccess = total;
                                        _order.Collected = 0;
                                        _order.IdPayment = 1;
                                        if (total > 50)
                                        {
                                            if (customer != null)
                                            {
                                                decimal diem = total / 50;
                                                int diem2 = Convert.ToInt32(diem);
                                                customer.scores = customer.scores + diem2;
                                                if (customer.scores >= 50 && customer.scores <= 100)
                                                {
                                                    customer.Idtype = 3;
                                                    _order.scores = 3;
                                                }
                                                else if (customer.scores >= 100 && customer.scores <= 500)
                                                {
                                                    customer.Idtype = 4;
                                                    _order.scores = 4;
                                                }
                                                else if (customer.scores >= 500)
                                                {
                                                    customer.Idtype = 5;
                                                    _order.scores = 5;
                                                }
                                                db.Entry(customer1).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                    if (customer.Idtype == 4)
                                    {
                                        db.OrderDetails.Add(_order_Detail);
                                        total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) / 15));
                                        _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                        _order.TotalSuccess = total;
                                        _order.Collected = 0;
                                        _order.IdPayment = 1;
                                        if (total > 50)
                                        {
                                            if (customer != null)
                                            {
                                                decimal diem = total / 50;
                                                int diem2 = Convert.ToInt32(diem);
                                                customer.scores = customer.scores + diem2;
                                                if (customer.scores >= 50 && customer.scores <= 100)
                                                {
                                                    customer.Idtype = 3;
                                                    _order.scores = 3;
                                                }
                                                else if (customer.scores >= 100 && customer.scores <= 500)
                                                {
                                                    customer.Idtype = 4;
                                                    _order.scores = 4;
                                                }
                                                else if (customer.scores >= 500)
                                                {
                                                    customer.Idtype = 5;
                                                    _order.scores = 5;
                                                }
                                                db.Entry(customer1).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                    if (customer.Idtype == 5)
                                    {
                                        db.OrderDetails.Add(_order_Detail);
                                        total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) / 20));
                                        _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                        _order.TotalSuccess = total;
                                        _order.Collected = 0;
                                        _order.IdPayment = 1;
                                        if (total > 50)
                                        {
                                            if (customer != null)
                                            {
                                                decimal diem = total / 50;
                                                int diem2 = Convert.ToInt32(diem);
                                                customer.scores = customer.scores + diem2;
                                                if (customer.scores >= 50 && customer.scores <= 100)
                                                {
                                                    customer.Idtype = 3;
                                                    _order.scores = 3;
                                                }
                                                else if (customer.scores >= 100 && customer.scores <= 500)
                                                {
                                                    customer.Idtype = 4;
                                                    _order.scores = 4;
                                                }
                                                else if (customer.scores >= 500)
                                                {
                                                    customer.Idtype = 5;
                                                    _order.scores = 5;
                                                }
                                                db.Entry(customer1).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    db.OrderDetails.Add(_order_Detail);
                                    total += (_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value);
                                    _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                    _order.TotalSuccess = total;
                                    _order.Collected = 0;
                                    _order.IdPayment = 1;
                                    if (total > 50)
                                    {
                                        if (customer != null)
                                        {
                                            decimal diem = total / 50;
                                            int diem2 = Convert.ToInt32(diem);
                                            customer.scores = customer.scores + diem2;
                                            if (customer.scores >= 50 && customer.scores <= 100)
                                            {
                                                customer.Idtype = 3;
                                                _order.scores = 3;
                                            }
                                            else if (customer.scores >= 100 && customer.scores <= 500)
                                            {
                                                customer.Idtype = 4;
                                                _order.scores = 4;
                                            }
                                            else if (customer.scores >= 500)
                                            {
                                                customer.Idtype = 5;
                                                _order.scores = 5;
                                            }
                                            db.Entry(customer1).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (customer.Idtype != null)
                                {
                                    if (customer.Idtype == 3)
                                    {
                                        _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                        db.OrderDetails.Add(_order_Detail);
                                        total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) / 10));
                                        _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                        _order.TotalSuccess = total;
                                        _order.Collected = 0;
                                        _order.IdPayment = 1;
                                        if (total > 50)
                                        {
                                            if (customer != null)
                                            {
                                                decimal diem = total / 50;
                                                int diem2 = Convert.ToInt32(diem);
                                                customer.scores = customer.scores + diem2;
                                                if (customer.scores >= 50 && customer.scores <= 100)
                                                {
                                                    customer.Idtype = 3;
                                                    _order.scores = 3;
                                                }
                                                else if (customer.scores >= 100 && customer.scores <= 500)
                                                {
                                                    customer.Idtype = 4;
                                                    _order.scores = 4;
                                                }
                                                else if (customer.scores >= 500)
                                                {
                                                    customer.Idtype = 5;
                                                    _order.scores = 5;
                                                }
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                    if (customer.Idtype == 4)
                                    {
                                        _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                        db.OrderDetails.Add(_order_Detail);
                                        total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) / 15));
                                        _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                        _order.TotalSuccess = total;
                                        _order.Collected = 0;
                                        _order.IdPayment = 1;
                                        if (total > 50)
                                        {
                                            if (customer != null)
                                            {
                                                decimal diem = total / 50;
                                                int diem2 = Convert.ToInt32(diem);
                                                customer.scores = customer.scores + diem2;
                                                if (customer.scores >= 50 && customer.scores <= 100)
                                                {
                                                    customer.Idtype = 3;
                                                    _order.scores = 3;
                                                }
                                                else if (customer.scores >= 100 && customer.scores <= 500)
                                                {
                                                    customer.Idtype = 4;
                                                    _order.scores = 4;
                                                }
                                                else if (customer.scores >= 500)
                                                {
                                                    customer.Idtype = 5;
                                                    _order.scores = 5;
                                                }
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                    if (customer.Idtype == 5)
                                    {
                                        _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                        db.OrderDetails.Add(_order_Detail);
                                        total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) / 20));
                                        _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                        _order.TotalSuccess = total;
                                        _order.Collected = 0;
                                        _order.IdPayment = 1;
                                        if (total > 50)
                                        {
                                            if (customer != null)
                                            {
                                                decimal diem = total / 50;
                                                int diem2 = Convert.ToInt32(diem);
                                                customer.scores = customer.scores + diem2;
                                                if (customer.scores >= 50 && customer.scores <= 100)
                                                {
                                                    customer.Idtype = 3;
                                                    _order.scores = 3;
                                                }
                                                else if (customer.scores >= 100 && customer.scores <= 500)
                                                {
                                                    customer.Idtype = 4;
                                                    _order.scores = 4;
                                                }
                                                else if (customer.scores >= 500)
                                                {
                                                    customer.Idtype = 5;
                                                    _order.scores = 5;
                                                }

                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                    db.OrderDetails.Add(_order_Detail);
                                    total += (_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value);
                                    _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                    _order.TotalSuccess = total;
                                    _order.Collected = 0;
                                    _order.IdPayment = 1;
                                    if (total > 50)
                                    {
                                        if (customer != null)
                                        {
                                            decimal diem = total / 50;
                                            int diem2 = Convert.ToInt32(diem);
                                            customer.scores = customer.scores + diem2;
                                            if (customer.scores >= 50 && customer.scores <= 100)
                                            {
                                                customer.Idtype = 3;
                                                _order.scores = 3;
                                            }
                                            else if (customer.scores >= 100 && customer.scores <= 500)
                                            {
                                                customer.Idtype = 4;
                                                _order.scores = 4;
                                            }
                                            else if (customer.scores >= 500)
                                            {
                                                customer.Idtype = 5;
                                                _order.scores = 5;
                                            }
                                            db.Entry(customer1).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }


                    }

                    string content = System.IO.File.ReadAllText(Server.MapPath("~/Static/user/neworder.html"));

                    content = content.Replace("{{CustomerName}}", _order.Customer.FirstName);
                    content = content.Replace("{{Phone}}", _order.SDT_Cus);
                    content = content.Replace("{{Email}}", _order.Email_Cus);
                    content = content.Replace("{{Address}}", _order.Descriptions);
                    content = content.Replace("{{Total}}", total.ToString("N0"));

                    var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
                    new Mail().SendMail(toEmail, "Đơn hàng mới từ Pheonix Bookstore", content);
                    var toEmails = _order.Customer.Email_Cus;
                    new Mail().SendMail(toEmails, "Ban co dơn hàng mới từ Pheonix Bookstore", content);
                    db.SaveChanges();
                    cart.ClearCart();
                    sessioncp = null;
                    Session.Add(Models.Common.CommonCp.SESSION_NAME, sessioncp);
                    return RedirectToAction("Shopping_Success", "ShoppingCart");
                }
                else
                {

                    Cart cart = Session["Cart"] as Cart;
                    var result = from r in db.Customers
                                 where r.CodeCus == session.UserID
                                 select r;
                    var cus2 = result.ToList().First();
                    _Order _order = new _Order();
                    _order.OrderDate = DateTime.Now;
                    _order.Email_Cus = cus2.Email_Cus;
                    _order.SDT_Cus = cus2.Phone_Cus;
                    _order.Password_cus = cus2.Password;
                    _order.Descriptions = cus2.Address_Cus;
                    _order.CodeCus = cus2.CodeCus;
                    _order.Deleted = false;
                    _order.Cancelled = false;
                    _order.Paid = false;
                    _order.Status = false;  db.Orders.Add(_order);

                    decimal total = 0;
                    foreach (var item in cart.Items)
                    {
                        OrderDetail _order_Detail = new OrderDetail();
                        _order_Detail.IDOrder = _order.IDOrder;
                        _order_Detail.IDProduct = item._shopping_product.IDProduct;
                        _order_Detail.UnitPriceSale = item._shopping_product.Price;
                        _order_Detail.QuantitySale = item._shopping_quantity;
                        _order_Detail.CreadeDay = DateTime.Now;
                        Product _pro = db.Products.Find(_order_Detail.IDProduct);
                        Customer customer1 = db.Customers.Find(cus2.CodeCus);

                        if (_pro == null)
                        {
                            if (customer1.Idtype != null)
                            {
                                if (customer1.Idtype == 3)
                                {
                                    db.OrderDetails.Add(_order_Detail);
                                    total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) / 10));
                                    _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                    _order.TotalSuccess = total;
                                    _order.Collected = 0;
                                    _order.IdPayment = 1;
                                    if (total > 50)
                                    {
                                        if (customer1 != null)
                                        {
                                            decimal diem = total / 50;
                                            int diem2 = Convert.ToInt32(diem);
                                            customer1.scores = customer1.scores + diem2;
                                            if (customer1.scores >= 50 && customer1.scores <= 100)
                                            {
                                                customer1.Idtype = 3;
                                                _order.scores = 3;
                                            }
                                            else if (customer1.scores >= 100 && customer1.scores <= 500)
                                            {
                                                customer1.Idtype = 4;
                                                _order.scores = 4;
                                            }
                                            else if (customer1.scores >= 500)
                                            {
                                                customer1.Idtype = 5;
                                                _order.scores = 5;
                                            }
                                            db.Entry(customer1).State = EntityState.Modified;

                                            db.SaveChanges();
                                        }
                                    }
                                }
                                if (customer1.Idtype == 4)
                                {
                                    db.OrderDetails.Add(_order_Detail);
                                    total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) / 15));
                                    _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                    _order.TotalSuccess = total;
                                    _order.Collected = 0;
                                    _order.IdPayment = 1;
                                    if (total > 50)
                                    {
                                        if (customer1 != null)
                                        {
                                            decimal diem = total / 50;
                                            int diem2 = Convert.ToInt32(diem);
                                            customer1.scores = customer1.scores + diem2;
                                            if (customer1.scores >= 50 && customer1.scores <= 100)
                                            {
                                                customer1.Idtype = 3;
                                                _order.scores = 3;
                                            }
                                            else if (customer1.scores >= 100 && customer1.scores <= 500)
                                            {
                                                customer1.Idtype = 4;
                                                _order.scores = 4;
                                            }
                                            else if (customer1.scores >= 500)
                                            {
                                                customer1.Idtype = 5;
                                                _order.scores = 5;
                                            }
                                            db.Entry(customer1).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                                if (customer1.Idtype == 5)
                                {
                                    db.OrderDetails.Add(_order_Detail);
                                    total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) / 20));
                                    _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                    _order.TotalSuccess = total;
                                    _order.Collected = 0;
                                    _order.IdPayment = 1;
                                    if (total > 50)
                                    {
                                        if (customer1 != null)
                                        {
                                            decimal diem = total / 50;
                                            int diem2 = Convert.ToInt32(diem);
                                            customer1.scores = customer1.scores + diem2;
                                            if (customer1.scores >= 50 && customer1.scores <= 100)
                                            {
                                                customer1.Idtype = 3;
                                                _order.scores = 3;
                                            }
                                            else if (customer1.scores >= 100 && customer1.scores <= 500)
                                            {
                                                customer1.Idtype = 4;
                                                _order.scores = 4;
                                            }
                                            else if (customer1.scores >= 500)
                                            {
                                                customer1.Idtype = 5;
                                                _order.scores = 5;
                                            }
                                            db.Entry(customer1).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                db.OrderDetails.Add(_order_Detail);
                                total += (_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value);
                                _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                _order.TotalSuccess = total;
                                _order.Collected = 0;
                                _order.IdPayment = 1;
                                if (total > 50)
                                {
                                    if (customer1 != null)
                                    {
                                        decimal diem = total / 50;
                                        int diem2 = Convert.ToInt32(diem);
                                        customer1.scores = customer1.scores + diem2;
                                        if (customer1.scores >= 50 && customer1.scores <= 100)
                                        {
                                            customer1.Idtype = 3;
                                            _order.scores = 3;
                                        }
                                        else if (customer1.scores >= 100 && customer1.scores <= 500)
                                        {
                                            customer1.Idtype = 4;
                                            _order.scores = 4;
                                        }
                                        else if (customer1.scores >= 500)
                                        {
                                            customer1.Idtype = 5;
                                            _order.scores = 5;
                                        }
                                        db.Entry(customer1).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (customer1.Idtype != null)
                            {
                                if (customer1.Idtype == 3)
                                {
                                    _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                    db.OrderDetails.Add(_order_Detail);
                                    total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) / 10));
                                    _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                    _order.TotalSuccess = total;
                                    _order.Collected = 0;
                                    _order.IdPayment = 1;
                                    if (total > 50)
                                    {
                                        if (customer1 != null)
                                        {
                                            decimal diem = total / 50;
                                            int diem2 = Convert.ToInt32(diem);
                                            customer1.scores = customer1.scores + diem2;
                                            if (customer1.scores >= 50 && customer1.scores <= 100)
                                            {
                                                customer1.Idtype = 3;
                                                _order.scores = 3;
                                            }
                                            else if (customer1.scores >= 100 && customer1.scores <= 500)
                                            {
                                                customer1.Idtype = 4;
                                                _order.scores = 4;
                                            }
                                            else if (customer1.scores >= 500)
                                            {
                                                customer1.Idtype = 5;
                                                _order.scores = 5;
                                            }
                                            db.Entry(customer1).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                                if (customer1.Idtype == 4)
                                {
                                    _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                    db.OrderDetails.Add(_order_Detail);
                                    total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) / 15));
                                    _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                    _order.TotalSuccess = total;
                                    _order.Collected = 0;
                                    _order.IdPayment = 1;
                                    if (total > 50)
                                    {
                                        if (customer1 != null)
                                        {
                                            decimal diem = total / 50;
                                            int diem2 = Convert.ToInt32(diem);
                                            customer1.scores = customer1.scores + diem2;
                                            if (customer1.scores >= 50 && customer1.scores <= 100)
                                            {
                                                customer1.Idtype = 3;
                                                _order.scores = 3;
                                            }
                                            else if (customer1.scores >= 100 && customer1.scores <= 500)
                                            {
                                                customer1.Idtype = 4;
                                                _order.scores = 4;
                                            }
                                            else if (customer1.scores >= 500)
                                            {
                                                customer1.Idtype = 5;
                                                _order.scores = 5;
                                            }
                                            db.Entry(customer1).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                                if (customer1.Idtype == 5)
                                {
                                    _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                    db.OrderDetails.Add(_order_Detail);
                                    total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) / 20));
                                    _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                    _order.TotalSuccess = total;
                                    _order.Collected = 0;
                                    _order.IdPayment = 1;
                                    if (total > 50)
                                    {
                                        if (customer1 != null)
                                        {
                                            decimal diem = total / 50;
                                            int diem2 = Convert.ToInt32(diem);
                                            customer1.scores = customer1.scores + diem2;
                                            if (customer1.scores >= 50 && customer1.scores <= 100)
                                            {
                                                customer1.Idtype = 3;
                                                _order.scores = 3;
                                            }
                                            else if (customer1.scores >= 100 && customer1.scores <= 500)
                                            {
                                                customer1.Idtype = 4;
                                                _order.scores = 4;
                                            }
                                            else if (customer1.scores >= 500)
                                            {
                                                customer1.Idtype = 5;
                                                _order.scores = 5;
                                            }
                                            db.Entry(customer1).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                db.OrderDetails.Add(_order_Detail);
                                total += (_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value);
                                _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                _order.TotalSuccess = total;
                                _order.Collected = 0;
                                _order.IdPayment = 1;
                                if (total > 50)
                                {
                                    if (customer1 != null)
                                    {
                                        decimal diem = total / 50;
                                        int diem2 = Convert.ToInt32(diem);
                                        customer1.scores = customer1.scores + diem2;
                                        if (customer1.scores >= 50 && customer1.scores <= 100)
                                        {
                                            customer1.Idtype = 3;
                                            _order.scores = 3;
                                        }
                                        else if (customer1.scores >= 100 && customer1.scores <= 500)
                                        {
                                            customer1.Idtype = 4;
                                            _order.scores = 4;
                                        }
                                        else if (customer1.scores >= 500)
                                        {
                                            customer1.Idtype = 5;
                                            _order.scores = 5;
                                        }
                                        db.Entry(customer1).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }

                    string content = System.IO.File.ReadAllText(Server.MapPath("~/Static/user/neworder.html"));

                    content = content.Replace("{{CustomerName}}", _order.Customer.FirstName);
                    content = content.Replace("{{Phone}}", _order.SDT_Cus);
                    content = content.Replace("{{Email}}", _order.Email_Cus);
                    content = content.Replace("{{Address}}", _order.Descriptions);
                    content = content.Replace("{{Total}}", total.ToString("N0"));

                    var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
                    new Mail().SendMail(toEmail, "Đơn hàng mới từ Pheonix Bookstore", content);
                    var toEmails = _order.Customer.Email_Cus;
                    new Mail().SendMail(toEmails, "Ban co dơn hàng mới từ Pheonix Bookstore", content);
                    db.SaveChanges();
                    cart.ClearCart();
                    sessioncp = null;
                    Session.Add(Models.Common.CommonCp.SESSION_NAME, sessioncp);
                    return RedirectToAction("Shopping_Success", "ShoppingCart");
                }

            }
        }

        public ActionResult CartShow()
        {
            Cart cart = Session["Cart"] as Cart;
            return View(cart);
        }

        public ActionResult CartShowMenu()
        {
            Cart cart = Session["Cart"] as Cart;
            return View(cart);
        }
        public ActionResult Update_Quantity_Cart(FormCollection form)
        {
            Cart cart = Session["Cart"] as Cart;
            int id_pro = int.Parse(form["ID_Product"]);

            Product _pro = db.Products.Find(id_pro);

            int quantity = int.Parse(form["Quantity"]);
            if (quantity > _pro.Quantity)
            {
                return RedirectToAction("Err", "ShoppingCart");
            }
            else
            {
                cart.Update_Quantity_Shopping(id_pro, quantity);
            }
            return RedirectToAction("ShowToCart", "ShoppingCart");
        }
        public ActionResult RemoveCart(int id)
        {
            Cart cart = Session["Cart"] as Cart;
            cart.Remove_Cart_Item(id);
            return RedirectToAction("ShowToCart", "ShoppingCart");
        }
        public PartialViewResult BagCart()
        {
            int _t_item = 0;
            Cart cart = Session["Cart"] as Cart;
            if (cart != null)
            {
                _t_item = cart.Total_Quantity();
            }
            ViewBag.infoCart = _t_item;
            return PartialView("BagCart");
        }
        public ActionResult Shopping_Success()
        {
            return View();
        }
        public ActionResult Err()
        {
            return View();
        }
        public ActionResult ErrCoupon()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ProceedOrder()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProceedOrder(Customer cus)
        {

            var check = db.Customers.Where(s => s.Email_Cus.Equals(cus.Email_Cus)
            && s.Password.Equals(cus.Password)).FirstOrDefault();
            if (check == null)
            {
                return RedirectToAction("ErrCoupon", "ShoppingCart");
            }
            else
            {
                try
                {
                    var result = from r in db.Customers
                                 where r.Email_Cus == cus.Email_Cus
                                 select r;
                    var cus2 = result.ToList().First();
                    var userSession = new UserLogin();
                    userSession.UserName = cus2.FirstName;
                    userSession.UserID = cus2.CodeCus;
                    Session.Add(Models.Common.CommonConstants.USER_SESSION, userSession);
                    return RedirectToAction("PurchaseAccount", "ShoppingCart");
                }
                catch
                {
                    return Content("Error checkout!!!!!");
                }

            }
        }

        public ActionResult ChangeAdress(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeAdress(Customer cus)
        {
            var sessioncp = (CouponSS)Session[Models.Common.CommonCp.SESSION_NAME];


            if (ModelState.IsValid)
            {
                if (sessioncp != null)
                {
                    string Tt = sessioncp.Name.ToString();
                    var res = (from r in db.Coupons
                               where (r.Name == Tt)
                               select r).AsEnumerable().FirstOrDefault();
                    Cart cart = Session["Cart"] as Cart;
                    _Order _order = new _Order();
                    _order.OrderDate = DateTime.Now;
                    _order.Email_Cus = cus.Email_Cus;
                    _order.SDT_Cus = cus.Phone_Cus;
                    _order.Password_cus = cus.Password;
                    _order.Descriptions = cus.Address_Cus;
                    _order.CodeCus = cus.CodeCus;
                    _order.Deleted = false;
                    _order.Cancelled = false;
                    _order.Paid = false;
                    _order.Status = false; db.Orders.Add(_order);

                    decimal total = 0;
                    foreach (var item in cart.Items)
                    {
                        OrderDetail _order_Detail = new OrderDetail();
                        _order_Detail.IDOrder = _order.IDOrder;
                        _order_Detail.IDProduct = item._shopping_product.IDProduct;
                        _order_Detail.UnitPriceSale = item._shopping_product.Price;
                        _order_Detail.QuantitySale = item._shopping_quantity;
                        _order_Detail.CreadeDay = DateTime.Now;
                        Product _pro = db.Products.Find(_order_Detail.IDProduct);
                        Customer customer1 = db.Customers.Find(cus.CodeCus);
                        if (res != null)
                        {
                            if (_pro == null)
                            {
                                if (customer1.Idtype != null)
                                {
                                    if (customer1.Idtype == 3)
                                    {
                                        db.OrderDetails.Add(_order_Detail);
                                        total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price) / 10));
                                        _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                        _order.TotalSuccess = total;
                                        _order.IdCoupon = res.IdCoupon;
                                        _order.Collected = 0;
                                        _order.IdPayment = 1;
                                        if (total > 50)
                                        {
                                            if (customer1 != null)
                                            {
                                                decimal diem = total / 50;
                                                int diem2 = Convert.ToInt32(diem);
                                                customer1.scores = customer1.scores + diem2;
                                                if (customer1.scores >= 50 && customer1.scores <= 100)
                                                {
                                                    customer1.Idtype = 3;
                                                    _order.scores = 3;
                                                }
                                                else if (customer1.scores >= 100 && customer1.scores <= 500)
                                                {
                                                    customer1.Idtype = 4;
                                                    _order.scores = 4;
                                                }
                                                else if (customer1.scores >= 500)
                                                {
                                                    customer1.Idtype = 5;
                                                    _order.scores = 5;
                                                }
                                                db.Entry(customer1).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                    if (customer1.Idtype == 4)
                                    {
                                        db.OrderDetails.Add(_order_Detail);
                                        total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price) / 15));
                                        _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                        _order.TotalSuccess = total;
                                        _order.IdCoupon = res.IdCoupon;
                                        _order.Collected = 0;
                                        _order.IdPayment = 1;
                                        if (total > 50)
                                        {
                                            if (customer1 != null)
                                            {
                                                decimal diem = total / 50;
                                                int diem2 = Convert.ToInt32(diem);
                                                customer1.scores = customer1.scores + diem2;
                                                if (customer1.scores >= 50 && customer1.scores <= 100)
                                                {
                                                    customer1.Idtype = 3;
                                                    _order.scores = 3;
                                                }
                                                else if (customer1.scores >= 100 && customer1.scores <= 500)
                                                {
                                                    customer1.Idtype = 4;
                                                    _order.scores = 4;
                                                }
                                                else if (customer1.scores >= 500)
                                                {
                                                    customer1.Idtype = 5;
                                                    _order.scores = 5;
                                                }
                                                db.Entry(customer1).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                    if (customer1.Idtype == 5)
                                    {
                                        db.OrderDetails.Add(_order_Detail);
                                        total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price) / 20));
                                        _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                        _order.TotalSuccess = total;
                                        _order.IdCoupon = res.IdCoupon;
                                        _order.Collected = 0;
                                        _order.IdPayment = 1;
                                        if (total > 50)
                                        {
                                            if (customer1 != null)
                                            {
                                                decimal diem = total / 50;
                                                int diem2 = Convert.ToInt32(diem);
                                                customer1.scores = customer1.scores + diem2;
                                                if (customer1.scores >= 50 && customer1.scores <= 100)
                                                {
                                                    customer1.Idtype = 3;
                                                    _order.scores = 3;
                                                }
                                                else if (customer1.scores >= 100 && customer1.scores <= 500)
                                                {
                                                    customer1.Idtype = 4;
                                                    _order.scores = 4;
                                                }
                                                else if (customer1.scores >= 500)
                                                {
                                                    customer1.Idtype = 5;
                                                    _order.scores = 5;
                                                }
                                                db.Entry(customer1).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    db.OrderDetails.Add(_order_Detail);
                                    total += (_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price;
                                    _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                    _order.TotalSuccess = total;
                                    _order.IdCoupon = res.IdCoupon;
                                    _order.Collected = 0;
                                    _order.IdPayment = 1;
                                    if (total > 50)
                                    {
                                        if (customer1 != null)
                                        {
                                            decimal diem = total / 50;
                                            int diem2 = Convert.ToInt32(diem);
                                            customer1.scores = customer1.scores + diem2;
                                            if (customer1.scores >= 50 && customer1.scores <= 100)
                                            {
                                                customer1.Idtype = 3;
                                                _order.scores = 3;
                                            }
                                            else if (customer1.scores >= 100 && customer1.scores <= 500)
                                            {
                                                customer1.Idtype = 4;
                                                _order.scores = 4;
                                            }
                                            else if (customer1.scores >= 500)
                                            {
                                                customer1.Idtype = 5;
                                                _order.scores = 5;
                                            }
                                            db.Entry(customer1).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }

                            }
                            else
                            {
                                if (customer1.Idtype != null)
                                {
                                    if (customer1.Idtype == 3)
                                    {
                                        _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                        db.OrderDetails.Add(_order_Detail);
                                        total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price) / 10));
                                        _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                        _order.TotalSuccess = total;
                                        _order.IdCoupon = res.IdCoupon;
                                        _order.Collected = 0;
                                        _order.IdPayment = 1;
                                        if (total > 50)
                                        {
                                            if (customer1 != null)
                                            {
                                                decimal diem = total / 50;
                                                int diem2 = Convert.ToInt32(diem);
                                                customer1.scores = customer1.scores + diem2;
                                                if (customer1.scores >= 50 && customer1.scores <= 100)
                                                {
                                                    customer1.Idtype = 3;
                                                    _order.scores = 3;
                                                }
                                                else if (customer1.scores >= 100 && customer1.scores <= 500)
                                                {
                                                    customer1.Idtype = 4;
                                                    _order.scores = 4;
                                                }
                                                else if (customer1.scores >= 500)
                                                {
                                                    customer1.Idtype = 5;
                                                    _order.scores = 5;
                                                }
                                                db.Entry(customer1).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                    if (customer1.Idtype == 4)
                                    {
                                        _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                        db.OrderDetails.Add(_order_Detail);
                                        total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price) / 15));
                                        _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                        _order.TotalSuccess = total;
                                        _order.IdCoupon = res.IdCoupon;
                                        _order.Collected = 0;
                                        _order.IdPayment = 1;
                                        if (total > 50)
                                        {
                                            if (customer1 != null)
                                            {
                                                decimal diem = total / 50;
                                                int diem2 = Convert.ToInt32(diem);
                                                customer1.scores = customer1.scores + diem2;
                                                if (customer1.scores >= 50 && customer1.scores <= 100)
                                                {
                                                    customer1.Idtype = 3;
                                                    _order.scores = 3;
                                                }
                                                else if (customer1.scores >= 100 && customer1.scores <= 500)
                                                {
                                                    customer1.Idtype = 4;
                                                    _order.scores = 4;
                                                }
                                                else if (customer1.scores >= 500)
                                                {
                                                    customer1.Idtype = 5;
                                                    _order.scores = 5;
                                                }
                                                db.Entry(customer1).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                    if (customer1.Idtype == 5)
                                    {
                                        _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                        db.OrderDetails.Add(_order_Detail);
                                        total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price) / 20));
                                        _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                        _order.TotalSuccess = total;
                                        _order.IdCoupon = res.IdCoupon;
                                        _order.Collected = 0;
                                        _order.IdPayment = 1;
                                        if (total > 50)
                                        {
                                            if (customer1 != null)
                                            {
                                                decimal diem = total / 50;
                                                int diem2 = Convert.ToInt32(diem);
                                                customer1.scores = customer1.scores + diem2;
                                                if (customer1.scores >= 50 && customer1.scores <= 100)
                                                {
                                                    customer1.Idtype = 3;
                                                    _order.scores = 3;
                                                }
                                                else if (customer1.scores >= 100 && customer1.scores <= 500)
                                                {
                                                    customer1.Idtype = 4;
                                                    _order.scores = 4;
                                                }
                                                else if (customer1.scores >= 500)
                                                {
                                                    customer1.Idtype = 5;
                                                    _order.scores = 5;
                                                }
                                                db.Entry(customer1).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                    db.OrderDetails.Add(_order_Detail);
                                    total += (_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value) - res.Price;
                                    _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                    _order.TotalSuccess = total;
                                    _order.IdCoupon = res.IdCoupon;
                                    _order.Collected = 0;
                                    _order.IdPayment = 1;
                                    if (total > 50)
                                    {
                                        if (customer1 != null)
                                        {
                                            decimal diem = total / 50;
                                            int diem2 = Convert.ToInt32(diem);
                                            customer1.scores = customer1.scores + diem2;
                                            if (customer1.scores >= 50 && customer1.scores <= 100)
                                            {
                                                customer1.Idtype = 3;
                                                _order.scores = 3;
                                            }
                                            else if (customer1.scores >= 100 && customer1.scores <= 500)
                                            {
                                                customer1.Idtype = 4;
                                                _order.scores = 4;
                                            }
                                            else if (customer1.scores >= 500)
                                            {
                                                customer1.Idtype = 5;
                                                _order.scores = 5;
                                            }
                                            db.Entry(customer1).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (_pro == null)
                            {

                                if (customer1.Idtype != null)
                                {
                                    if (customer1.Idtype == 3)
                                    {
                                        db.OrderDetails.Add(_order_Detail);
                                        total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) / 10));
                                        _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                        _order.TotalSuccess = total;
                                        _order.Collected = 0;
                                        _order.IdPayment = 1;
                                        if (total > 50)
                                        {
                                            if (customer1 != null)
                                            {
                                                decimal diem = total / 50;
                                                int diem2 = Convert.ToInt32(diem);
                                                customer1.scores = customer1.scores + diem2;
                                                if (customer1.scores >= 50 && customer1.scores <= 100)
                                                {
                                                    customer1.Idtype = 3;
                                                    _order.scores = 3;
                                                }
                                                else if (customer1.scores >= 100 && customer1.scores <= 500)
                                                {
                                                    customer1.Idtype = 4;
                                                    _order.scores = 4;
                                                }
                                                else if (customer1.scores >= 500)
                                                {
                                                    customer1.Idtype = 5;
                                                    _order.scores = 5;
                                                }
                                                db.Entry(customer1).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                    if (customer1.Idtype == 4)
                                    {
                                        db.OrderDetails.Add(_order_Detail);
                                        total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) / 15));
                                        _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                        _order.TotalSuccess = total;
                                        _order.Collected = 0;
                                        _order.IdPayment = 1;
                                        if (total > 50)
                                        {
                                            if (customer1 != null)
                                            {
                                                decimal diem = total / 50;
                                                int diem2 = Convert.ToInt32(diem);
                                                customer1.scores = customer1.scores + diem2;
                                                if (customer1.scores >= 50 && customer1.scores <= 100)
                                                {
                                                    customer1.Idtype = 3;
                                                    _order.scores = 3;
                                                }
                                                else if (customer1.scores >= 100 && customer1.scores <= 500)
                                                {
                                                    customer1.Idtype = 4;
                                                    _order.scores = 4;
                                                }
                                                else if (customer1.scores >= 500)
                                                {
                                                    customer1.Idtype = 5;
                                                    _order.scores = 5;
                                                }
                                                db.Entry(customer1).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                    if (customer1.Idtype == 5)
                                    {
                                        db.OrderDetails.Add(_order_Detail);
                                        total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) / 20));
                                        _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                        _order.TotalSuccess = total;
                                        _order.Collected = 0;
                                        _order.IdPayment = 1;
                                        if (total > 50)
                                        {
                                            if (customer1 != null)
                                            {
                                                decimal diem = total / 50;
                                                int diem2 = Convert.ToInt32(diem);
                                                customer1.scores = customer1.scores + diem2;
                                                if (customer1.scores >= 50 && customer1.scores <= 100)
                                                {
                                                    customer1.Idtype = 3;
                                                    _order.scores = 3;
                                                }
                                                else if (customer1.scores >= 100 && customer1.scores <= 500)
                                                {
                                                    customer1.Idtype = 4;
                                                    _order.scores = 4;
                                                }
                                                else if (customer1.scores >= 500)
                                                {
                                                    customer1.Idtype = 5;
                                                    _order.scores = 5;
                                                }
                                                db.Entry(customer1).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    db.OrderDetails.Add(_order_Detail);
                                    total += (_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value);
                                    _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                    _order.TotalSuccess = total;
                                    _order.Collected = 0;
                                    _order.IdPayment = 1;
                                    if (total > 50)
                                    {
                                        if (customer1 != null)
                                        {
                                            decimal diem = total / 50;
                                            int diem2 = Convert.ToInt32(diem);
                                            customer1.scores = customer1.scores + diem2;
                                            if (customer1.scores >= 50 && customer1.scores <= 100)
                                            {
                                                customer1.Idtype = 3;
                                                _order.scores = 3;
                                            }
                                            else if (customer1.scores >= 100 && customer1.scores <= 500)
                                            {
                                                customer1.Idtype = 4;
                                                _order.scores = 4;
                                            }
                                            else if (customer1.scores >= 500)
                                            {
                                                customer1.Idtype = 5;
                                                _order.scores = 5;
                                            }
                                            db.Entry(customer1).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (customer1.Idtype != null)
                                {
                                    if (customer1.Idtype == 3)
                                    {
                                        _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                        db.OrderDetails.Add(_order_Detail);
                                        total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) / 10));
                                        _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                        _order.TotalSuccess = total;
                                        _order.Collected = 0;
                                        _order.IdPayment = 1;
                                        if (total > 50)
                                        {
                                            if (customer1 != null)
                                            {
                                                decimal diem = total / 50;
                                                int diem2 = Convert.ToInt32(diem);
                                                customer1.scores = customer1.scores + diem2;
                                                if (customer1.scores >= 50 && customer1.scores <= 100)
                                                {
                                                    customer1.Idtype = 3;
                                                    _order.scores = 3;
                                                }
                                                else if (customer1.scores >= 100 && customer1.scores <= 500)
                                                {
                                                    customer1.Idtype = 4;
                                                    _order.scores = 4;
                                                }
                                                else if (customer1.scores >= 500)
                                                {
                                                    customer1.Idtype = 5;
                                                    _order.scores = 5;
                                                }
                                                db.Entry(customer1).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                    if (customer1.Idtype == 4)
                                    {
                                        _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                        db.OrderDetails.Add(_order_Detail);
                                        total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) / 15));
                                        _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                        _order.TotalSuccess = total;
                                        _order.Collected = 0;
                                        _order.IdPayment = 1;
                                        if (total > 50)
                                        {
                                            if (customer1 != null)
                                            {
                                                decimal diem = total / 50;
                                                int diem2 = Convert.ToInt32(diem);
                                                customer1.scores = customer1.scores + diem2;
                                                if (customer1.scores >= 50 && customer1.scores <= 100)
                                                {
                                                    customer1.Idtype = 3;
                                                    _order.scores = 3;
                                                }
                                                else if (customer1.scores >= 100 && customer1.scores <= 500)
                                                {
                                                    customer1.Idtype = 4;
                                                    _order.scores = 4;
                                                }
                                                else if (customer1.scores >= 500)
                                                {
                                                    customer1.Idtype = 5;
                                                    _order.scores = 5;
                                                }
                                                db.Entry(customer1).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                    if (customer1.Idtype == 5)
                                    {
                                        _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                        db.OrderDetails.Add(_order_Detail);
                                        total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) / 20));
                                        _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                        _order.TotalSuccess = total;
                                        _order.Collected = 0;
                                        _order.IdPayment = 1;
                                        if (total > 50)
                                        {
                                            if (customer1 != null)
                                            {
                                                decimal diem = total / 50;
                                                int diem2 = Convert.ToInt32(diem);
                                                customer1.scores = customer1.scores + diem2;
                                                if (customer1.scores >= 50 && customer1.scores <= 100)
                                                {
                                                    customer1.Idtype = 3;
                                                    _order.scores = 3;
                                                }
                                                else if (customer1.scores >= 100 && customer1.scores <= 500)
                                                {
                                                    customer1.Idtype = 4;
                                                    _order.scores = 4;
                                                }
                                                else if (customer1.scores >= 500)
                                                {
                                                    customer1.Idtype = 5;
                                                    _order.scores = 5;
                                                }
                                                db.Entry(customer1).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                    db.OrderDetails.Add(_order_Detail);
                                    total += (_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value);
                                    _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                    _order.TotalSuccess = total;
                                    _order.Collected = 0;
                                    _order.IdPayment = 1;
                                    if (total > 50)
                                    {
                                        if (customer1 != null)
                                        {
                                            decimal diem = total / 50;
                                            int diem2 = Convert.ToInt32(diem);
                                            customer1.scores = customer1.scores + diem2;
                                            if (customer1.scores >= 50 && customer1.scores <= 100)
                                            {
                                                customer1.Idtype = 3;
                                                _order.scores = 3;
                                            }
                                            else if (customer1.scores >= 100 && customer1.scores <= 500)
                                            {
                                                customer1.Idtype = 4;
                                                _order.scores = 4;
                                            }
                                            else if (customer1.scores >= 500)
                                            {
                                                customer1.Idtype = 5;
                                                _order.scores = 5;
                                            }
                                            db.Entry(customer1).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }


                    }

                    string content = System.IO.File.ReadAllText(Server.MapPath("~/Static/user/neworder.html"));

                    content = content.Replace("{{CustomerName}}", _order.Customer.FirstName);
                    content = content.Replace("{{Phone}}", _order.SDT_Cus);
                    content = content.Replace("{{Email}}", _order.Email_Cus);
                    content = content.Replace("{{Address}}", _order.Descriptions);
                    content = content.Replace("{{Total}}", total.ToString("N0"));

                    var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
                    new Mail().SendMail(toEmail, "Đơn hàng mới từ Pheonix Bookstore", content);
                    var toEmails = _order.Customer.Email_Cus;
                    new Mail().SendMail(toEmails, "Ban co dơn hàng mới từ Pheonix Bookstore", content);
                    db.SaveChanges();
                    cart.ClearCart();
                    sessioncp = null;
                    Session.Add(Models.Common.CommonCp.SESSION_NAME, sessioncp);
                    return RedirectToAction("Shopping_Success", "ShoppingCart");
                }
                else
                {

                    Cart cart = Session["Cart"] as Cart;
                    _Order _order = new _Order();
                    _order.OrderDate = DateTime.Now;
                    _order.Email_Cus = cus.Email_Cus;
                    _order.SDT_Cus = cus.Phone_Cus;
                    _order.Password_cus = cus.Password;
                    _order.Descriptions = cus.Address_Cus;
                    _order.CodeCus = cus.CodeCus;
                    _order.Deleted = false;
                    _order.Cancelled = false;
                    _order.Paid = false;
                    _order.Status = false; db.Orders.Add(_order);

                    decimal total = 0;
                    foreach (var item in cart.Items)
                    {
                        OrderDetail _order_Detail = new OrderDetail();
                        _order_Detail.IDOrder = _order.IDOrder;
                        _order_Detail.IDProduct = item._shopping_product.IDProduct;
                        _order_Detail.UnitPriceSale = item._shopping_product.Price;
                        _order_Detail.QuantitySale = item._shopping_quantity;
                        _order_Detail.CreadeDay = DateTime.Now;
                        Product _pro = db.Products.Find(_order_Detail.IDProduct);
                        Customer customer1 = db.Customers.Find(cus.CodeCus);

                        if (_pro == null)
                        {
                            if (customer1.Idtype != null)
                            {
                                if (customer1.Idtype == 3)
                                {
                                    db.OrderDetails.Add(_order_Detail);
                                    total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) / 10));
                                    _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                    _order.TotalSuccess = total;
                                    _order.Collected = 0;
                                    _order.IdPayment = 1;
                                    if (total > 50)
                                    {
                                        if (customer1 != null)
                                        {
                                            decimal diem = total / 50;
                                            int diem2 = Convert.ToInt32(diem);
                                            customer1.scores = customer1.scores + diem2;
                                            if (customer1.scores >= 50 && customer1.scores <= 100)
                                            {
                                                customer1.Idtype = 3;
                                                _order.scores = 3;
                                            }
                                            else if (customer1.scores >= 100 && customer1.scores <= 500)
                                            {
                                                customer1.Idtype = 4;
                                                _order.scores = 4;
                                            }
                                            else if (customer1.scores >= 500)
                                            {
                                                customer1.Idtype = 5;
                                                _order.scores = 5;
                                            }
                                            db.Entry(customer1).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                                if (customer1.Idtype == 4)
                                {
                                    db.OrderDetails.Add(_order_Detail);
                                    total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) / 15));
                                    _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                    _order.TotalSuccess = total;
                                    _order.Collected = 0;
                                    _order.IdPayment = 1;
                                    if (total > 50)
                                    {
                                        if (customer1 != null)
                                        {
                                            decimal diem = total / 50;
                                            int diem2 = Convert.ToInt32(diem);
                                            customer1.scores = customer1.scores + diem2;
                                            if (customer1.scores >= 50 && customer1.scores <= 100)
                                            {
                                                customer1.Idtype = 3;
                                                _order.scores = 3;
                                            }
                                            else if (customer1.scores >= 100 && customer1.scores <= 500)
                                            {
                                                customer1.Idtype = 4;
                                                _order.scores = 4;
                                            }
                                            else if (customer1.scores >= 500)
                                            {
                                                customer1.Idtype = 5;
                                                _order.scores = 5;
                                            }
                                            db.Entry(customer1).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                                if (customer1.Idtype == 5)
                                {
                                    db.OrderDetails.Add(_order_Detail);
                                    total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) / 20));
                                    _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                    _order.TotalSuccess = total;
                                    _order.Collected = 0;
                                    _order.IdPayment = 1;
                                    if (total > 50)
                                    {
                                        if (customer1 != null)
                                        {
                                            decimal diem = total / 50;
                                            int diem2 = Convert.ToInt32(diem);
                                            customer1.scores = customer1.scores + diem2;
                                            if (customer1.scores >= 50 && customer1.scores <= 100)
                                            {
                                                customer1.Idtype = 3;
                                                _order.scores = 3;
                                            }
                                            else if (customer1.scores >= 100 && customer1.scores <= 500)
                                            {
                                                customer1.Idtype = 4;
                                                _order.scores = 4;
                                            }
                                            else if (customer1.scores >= 500)
                                            {
                                                customer1.Idtype = 5;
                                                _order.scores = 5;
                                            }
                                            db.Entry(customer1).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                db.OrderDetails.Add(_order_Detail);
                                total += (_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value);
                                _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                _order.TotalSuccess = total;
                                _order.Collected = 0;
                                _order.IdPayment = 1;
                                if (total > 50)
                                {
                                    if (customer1 != null)
                                    {
                                        decimal diem = total / 50;
                                        int diem2 = Convert.ToInt32(diem);
                                        customer1.scores = customer1.scores + diem2;
                                        if (customer1.scores >= 50 && customer1.scores <= 100)
                                        {
                                            customer1.Idtype = 3;
                                            _order.scores = 3;
                                        }
                                        else if (customer1.scores >= 100 && customer1.scores <= 500)
                                        {
                                            customer1.Idtype = 4;
                                            _order.scores = 4;
                                        }
                                        else if (customer1.scores >= 500)
                                        {
                                            customer1.Idtype = 5;
                                            _order.scores = 5;
                                        }
                                        db.Entry(customer1).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (customer1.Idtype != null)
                            {
                                if (customer1.Idtype == 3)
                                {
                                    _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                    db.OrderDetails.Add(_order_Detail);
                                    total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) / 10));
                                    _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                    _order.TotalSuccess = total;
                                    _order.Collected = 0;
                                    _order.IdPayment = 1;
                                    if (total > 50)
                                    {
                                        if (customer1 != null)
                                        {
                                            decimal diem = total / 50;
                                            int diem2 = Convert.ToInt32(diem);
                                            customer1.scores = customer1.scores + diem2;
                                            if (customer1.scores >= 50 && customer1.scores <= 100)
                                            {
                                                customer1.Idtype = 3;
                                                _order.scores = 3;
                                            }
                                            else if (customer1.scores >= 100 && customer1.scores <= 500)
                                            {
                                                customer1.Idtype = 4;
                                                _order.scores = 4;
                                            }
                                            else if (customer1.scores >= 500)
                                            {
                                                customer1.Idtype = 5;
                                                _order.scores = 5;
                                            }
                                            db.Entry(customer1).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                                if (customer1.Idtype == 4)
                                {
                                    _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                    db.OrderDetails.Add(_order_Detail);
                                    total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) / 15));
                                    _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                    _order.TotalSuccess = total;
                                    _order.Collected = 0;
                                    _order.IdPayment = 1;
                                    if (total > 50)
                                    {
                                        if (customer1 != null)
                                        {
                                            decimal diem = total / 50;
                                            int diem2 = Convert.ToInt32(diem);
                                            customer1.scores = customer1.scores + diem2;
                                            if (customer1.scores >= 50 && customer1.scores <= 100)
                                            {
                                                customer1.Idtype = 3;
                                                _order.scores = 3;
                                            }
                                            else if (customer1.scores >= 100 && customer1.scores <= 500)
                                            {
                                                customer1.Idtype = 4;
                                                _order.scores = 4;
                                            }
                                            else if (customer1.scores >= 500)
                                            {
                                                customer1.Idtype = 5;
                                                _order.scores = 5;
                                            }
                                            db.Entry(customer1).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                                if (customer1.Idtype == 5)
                                {
                                    _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                    db.OrderDetails.Add(_order_Detail);
                                    total += (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) - (((_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value)) / 20));
                                    _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                    _order.TotalSuccess = total;
                                    _order.Collected = 0;
                                    _order.IdPayment = 1;
                                    if (total > 50)
                                    {
                                        if (customer1 != null)
                                        {
                                            decimal diem = total / 50;
                                            int diem2 = Convert.ToInt32(diem);
                                            customer1.scores = customer1.scores + diem2;
                                            if (customer1.scores >= 50 && customer1.scores <= 100)
                                            {
                                                customer1.Idtype = 3;
                                                _order.scores = 3;
                                            }
                                            else if (customer1.scores >= 100 && customer1.scores <= 500)
                                            {
                                                customer1.Idtype = 4;
                                                _order.scores = 4;
                                            }
                                            else if (customer1.scores >= 500)
                                            {
                                                customer1.Idtype = 5;
                                                _order.scores = 5;
                                            }
                                            db.Entry(customer1).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                db.OrderDetails.Add(_order_Detail);
                                total += (_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value);
                                _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                _order.TotalSuccess = total;
                                _order.Collected = 0;
                                _order.IdPayment = 1;
                                if (total > 50)
                                {
                                    if (customer1 != null)
                                    {
                                        decimal diem = total / 50;
                                        int diem2 = Convert.ToInt32(diem);
                                        customer1.scores = customer1.scores + diem2;
                                        if (customer1.scores >= 50 && customer1.scores <= 100)
                                        {
                                            customer1.Idtype = 3;
                                            _order.scores = 3;
                                        }
                                        else if (customer1.scores >= 100 && customer1.scores <= 500)
                                        {
                                            customer1.Idtype = 4;
                                            _order.scores = 4;
                                        }
                                        else if (customer1.scores >= 500)
                                        {
                                            customer1.Idtype = 5;
                                            _order.scores = 5;
                                        }
                                        db.Entry(customer1).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }

                    string content = System.IO.File.ReadAllText(Server.MapPath("~/Static/user/neworder.html"));

                    content = content.Replace("{{CustomerName}}", cus.FirstName);
                    content = content.Replace("{{Phone}}", cus.Phone_Cus);
                    content = content.Replace("{{Email}}", cus.Email_Cus);
                    content = content.Replace("{{Address}}", cus.Address_Cus);
                    content = content.Replace("{{Total}}", total.ToString("N0"));

                    var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
                    new Mail().SendMail(toEmail, "Đơn hàng mới từ Pheonix Bookstore", content);
                    var toEmails = _order.Customer.Email_Cus;
                    new Mail().SendMail(toEmails, "Ban co dơn hàng mới từ Pheonix Bookstore", content);
                    db.SaveChanges();
                    cart.ClearCart();
                    sessioncp = null;
                    Session.Add(Models.Common.CommonCp.SESSION_NAME, sessioncp);
                    return RedirectToAction("Shopping_Success", "ShoppingCart");
                }
                //_order.OrderDate = DateTime.Now;
                //_order.Email_Cus = customer.Email_Cus;
                //_order.SDT_Cus = customer.Phone_Cus;
                //_order.Password_cus = customer.Password;
                //_order.Descriptions = customer.Address_Cus;
                //_order.CodeCus = customer.CodeCus;
                //_order.Deleted = false;
                //_order.Cancelled = false;
                //_order.Paid = false;
                //_order.Status = false;

                //
                //foreach (var item in cart.Items)
                //{
                //    OrderDetail _order_Detail = new OrderDetail();
                //    _order_Detail.IDOrder = _order.IDOrder;
                //    _order_Detail.IDProduct = item._shopping_product.IDProduct;
                //    _order_Detail.UnitPriceSale = item._shopping_product.Price;
                //    _order_Detail.QuantitySale = item._shopping_quantity;

                //    Product _pro = db.Products.Find(_order_Detail.IDProduct);
                //    if (_pro == null)
                //    {
                //        db.OrderDetails.Add(_order_Detail);
                //    }
                //    else
                //    {
                //        _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                //        db.OrderDetails.Add(_order_Detail);
                //    }
                //}
                //db.Orders.Add(_order);string content = System.IO.File.ReadAllText(Server.MapPath("~/Static/user/neworder.html"));

                //content = content.Replace("{{CustomerName}}", _order.Customer.FirstName);
                //content = content.Replace("{{Phone}}", _order.SDT_Cus);
                //content = content.Replace("{{Email}}", _order.Email_Cus);
                //content = content.Replace("{{Address}}", _order.Descriptions);

                //var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
                //new Mail().SendMail(toEmail, "Đơn hàng mới từ Pheonix Bookstore", content);
                //var toEmails = _order.Customer.Email_Cus;
                //new Mail().SendMail(toEmails, "Ban co dơn hàng mới từ Pheonix Bookstore", content);


                //db.SaveChanges();
                //cart.ClearCart();
                //return RedirectToAction("Shopping_Success", "ShoppingCart");
            }
            return View(cus);
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("IndexUser", "Home");
        }
        //---------------------------paypal----------------------------------- cái class Order với Payment của m trùng với class thư viện của paypal nên t sửa lại _Order với _Payment rồi :))
        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            var paypaloder = DateTime.Now.Ticks;
            double tongtien = 0;
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            Cart cart = Session["Cart"] as Cart;
            foreach (var item in cart.Items)
            {
                itemList.items.Add(new Item()
                {
                    name = item._shopping_product.ProductName,
                    currency = "USD",
                    price = item._shopping_product.PromotionPrice.ToString(),// giá này nếu làm tính khác thì sửa lại, này t lấy giá promo
                    quantity = item._shopping_quantity.ToString(),
                    sku = "sku"
                });
                tongtien += double.Parse((item._shopping_product.PromotionPrice * item._shopping_quantity).ToString());
            }

            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            var details = new Details()
            {
                tax = "0",
                shipping = "25",// nếu m có làm ship thì sửa chổ này lại, này t làm mặc định luôn
                subtotal = tongtien.ToString()
            };
            var amount = new Amount()
            {
                currency = "USD",// tỷ giá của paypal chỉ có usd thôi chứ k có vnd nên nếu m làm vnd thì tính tỷ giá chổ tổng giá tiền lại
                total = (tongtien + double.Parse(details.shipping)).ToString(),
                details = details
            };
            var transactionList = new List<Transaction>();
            transactionList.Add(new Transaction()
            {
                description = $"Invoice #{paypaloder}",
                invoice_number = paypaloder.ToString(),
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            return this.payment.Create(apiContext);
        }
        public ActionResult PayMent()
        {
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/ShoppingCart/PayMent?";
                    var guid = Convert.ToString((new Random()).Next(100000));
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return RedirectToAction("CheckoutFail");
                    }

                    var session = (UserLogin)Session[Models.Common.CommonConstants.USER_SESSION];
                    if (session == null)
                    {
                        return Content("Hay dang nhap trươc khi mua truc tuyen paypal");

                    }
                    else
                    {
                        Cart cart = Session["Cart"] as Cart;
                        var result = from r in db.Customers
                                     where r.CodeCus == session.UserID
                                     select r;
                        var cus2 = result.ToList().First();
                        _Order _order = new _Order();
                        _order.OrderDate = DateTime.Now;
                        _order.Email_Cus = cus2.Email_Cus;
                        _order.SDT_Cus = cus2.Phone_Cus;
                        _order.Password_cus = cus2.Password;
                        _order.Descriptions = cus2.Address_Cus;
                        _order.CodeCus = cus2.CodeCus;
                        _order.Deleted = false;
                        _order.Cancelled = false;
                        _order.Paid = false;
                        _order.Status = false;  db.Orders.Add(_order);
                        decimal total = 0;
                        foreach (var item in cart.Items)
                        {
                            OrderDetail _order_Detail = new OrderDetail();
                            _order_Detail.IDOrder = _order.IDOrder;
                            _order_Detail.IDProduct = item._shopping_product.IDProduct;
                            _order_Detail.UnitPriceSale = item._shopping_product.Price;
                            _order_Detail.QuantitySale = item._shopping_quantity;
                            _order_Detail.CreadeDay = DateTime.Now;
                            Product _pro = db.Products.Find(_order_Detail.IDProduct);
                            if (_pro == null)
                            {
                                db.OrderDetails.Add(_order_Detail);
                                total += (_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value);
                                _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                _order.TotalSuccess = total;
                                _order.Collected = 0;
                                _order.IdPayment = 2;
                            }
                            else
                            {
                                _pro.Quantity = _pro.Quantity - _order_Detail.QuantitySale;
                                db.OrderDetails.Add(_order_Detail);
                                total += (_order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value);
                                _order.Total = _order_Detail.UnitPriceSale.Value * _order_Detail.QuantitySale.Value;
                                _order.TotalSuccess = total;
                                _order.Collected = 0;
                                _order.IdPayment = 2;
                            }

                        }

                        string content = System.IO.File.ReadAllText(Server.MapPath("~/Static/user/neworder.html"));

                        content = content.Replace("{{CustomerName}}", _order.Customer.FirstName);
                        content = content.Replace("{{Phone}}", _order.SDT_Cus);
                        content = content.Replace("{{Email}}", _order.Email_Cus);
                        content = content.Replace("{{Address}}", _order.Descriptions);
                        content = content.Replace("{{Total}}", total.ToString("N0"));

                        var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
                        new Mail().SendMail(toEmail, "Đơn hàng mới từ Pheonix Bookstore", content);
                        var toEmails = _order.Customer.Email_Cus;
                        new Mail().SendMail(toEmails, "Ban co dơn hàng mới từ Pheonix Bookstore", content);
                        db.SaveChanges();
                        cart.ClearCart();
                        return RedirectToAction("Shopping_Success", "ShoppingCart");

                    }
                }
            }
            catch
            {
                return View("FailureView");
            }
        }
    }
}