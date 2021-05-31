using Doan.Models;
using Doan.Models.ClassLibrary;
using Doan.Models.Common;
using Doan.Models.Dao;
using Doan.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using History = Doan.Models.ViewModel.History;

namespace Doan.Controllers
{
    public class CustomersController : Controller
    {
        // GET: Customers
        Db_Doan db = new Db_Doan();
        public ActionResult Index()
        {
            return View(db.Customers.ToList());
        }

        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            Customer cus = db.Customers.Find(id);
            if (cus == null)
            {
                return HttpNotFound();
            }
            return View(cus);
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Customer cus)
        {

                var check = db.Customers.FirstOrDefault(s => s.Email_Cus == cus.Email_Cus);
                if (check == null)
                {
                    cus.CreatedDay = DateTime.Now;
                    cus.scores = 0;
                    db.Customers.Add(cus);
                    db.SaveChanges();
                    if (cus.Password != null)
                    {
                        return RedirectToAction("Order_Success", "ShoppingCart");
                    }
                    else
                    {
                    var sessioncp = (CouponSS)Session[Models.Common.CommonCp.SESSION_NAME];
                    if (sessioncp != null)
                    {
                        string Tt = sessioncp.Name.ToString();
                        var res = (from r in db.Coupons
                                   where (r.Name == Tt)
                                   select r).AsEnumerable().FirstOrDefault();
                        Cart cart = Session["Cart"] as Cart;
                        var result = from r in db.Customers
                                     where r.CodeCus == cus.CodeCus
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
                                                    db.Entry(customer).State = EntityState.Modified;
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
                                                    db.Entry(customer).State = EntityState.Modified;
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
                                                    db.Entry(customer).State = EntityState.Modified;
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
                                                db.Entry(customer).State = EntityState.Modified;
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
                                                    db.Entry(customer).State = EntityState.Modified;
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
                                                    db.Entry(customer).State = EntityState.Modified;
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
                                                    db.Entry(customer).State = EntityState.Modified;
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
                                                db.Entry(customer).State = EntityState.Modified;
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
                                                    db.Entry(customer).State = EntityState.Modified;
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
                                                    db.Entry(customer).State = EntityState.Modified;
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
                                                    db.Entry(customer).State = EntityState.Modified;
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
                                                db.Entry(customer).State = EntityState.Modified;
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
                                                    db.Entry(customer).State = EntityState.Modified;
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
                                                    db.Entry(customer).State = EntityState.Modified;
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
                                                    db.Entry(customer).State = EntityState.Modified;
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
                                                db.Entry(customer).State = EntityState.Modified;
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
                                     where r.CodeCus == cus.CodeCus
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
                            Customer customer1 = db.Customers.Find(cus2.CodeCus);
                            Customer customer = db.Customers.Where(n => n.IDCus.Equals(customer1.IDCus)).FirstOrDefault();

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
                                                db.Entry(customer).State = EntityState.Modified;
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
                                                db.Entry(customer).State = EntityState.Modified;
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
                                                db.Entry(customer).State = EntityState.Modified;
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
                                            db.Entry(customer).State = EntityState.Modified;
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
                                                db.Entry(customer).State = EntityState.Modified;
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
                                                db.Entry(customer).State = EntityState.Modified;
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
                                                db.Entry(customer).State = EntityState.Modified;
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
                                            db.Entry(customer).State = EntityState.Modified;
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
                else
                {
                    return RedirectToAction("ErrCoupon", "ShoppingCart");
                }
            //try
            //{
            //    if (ModelState.IsValid)
            //    {
            //        db.Customers.Add(cus);
            //        db.SaveChanges();
            //        return RedirectToAction("ShowToCart", "ShoppingCart");
            //    }
            //    return View();
            //}
            //catch
            //{
            //    return Content("Error");
            //}
        }
        public ActionResult Edit(long? id)
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

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Customer customer)
        {
            var cus = db.Customers.AsNoTracking().Where(x => x.CodeCus == customer.CodeCus).FirstOrDefault();
            if (cus!= null)
            {
                var customer2 = db.Customers.AsNoTracking().Where(x => x.CodeCus== customer.CodeCus).FirstOrDefault();
                if(customer.Password == null)
                {
                    cus.Password = customer2.Password;
                }
                if (customer.CreatedDay == null)
                {
                    cus.CreatedDay = customer2.CreatedDay;
                }
                cus.LastName = customer.LastName;
                cus.FirstName = customer.FirstName;
                cus.Address_Cus = customer.Address_Cus;
                cus.Phone_Cus = customer.Phone_Cus;
                cus.ModifiedDay = DateTime.Now;
                cus.scores = customer2.scores;
                cus.Idtype = customer2.Idtype;
                db.Entry(cus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("/Details/"+ cus.CodeCus);
            }
            return Content("err");
        }

        public ActionResult ChangePass()
        {
            return View();
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePass(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var customer2 = db.Customers.AsNoTracking().Where(x => x.CodeCus == customer.CodeCus).FirstOrDefault();
                if (customer2.Password != customer.Password)
                {
                    return Content("Pass sai, hay thu nhap lai");
                }
                else if( customer.NewPassword != customer.ConfirmPassword)
                {
                    return Content("Pass khoong match vs confirm pass, hay nhap lai");
                }
                else
                {
                    customer.Email_Cus = customer2.Email_Cus;
                    customer.Phone_Cus = customer2.Phone_Cus;
                    customer.Address_Cus = customer2.Address_Cus;
                    customer.FirstName = customer2.FirstName;
                    customer.LastName = customer2.LastName;
                    customer.CreatedDay = customer2.CreatedDay;
                    customer.Password = customer.NewPassword;
                    db.Entry(customer).State = EntityState.Modified;
                }
                db.SaveChanges();
                return RedirectToAction("/Details/" + customer.CodeCus);
               
            }
            return View(customer);
        }


         //var check = db.Customers.Where(s => s.Email_Cus.Equals(cus.Email_Cus)
         //   && s.Password.Equals(cus.Password)).FirstOrDefault();
         //   if (check == null)
         //   {
         //       return RedirectToAction("ErrCoupon", "ShoppingCart");
         //   }
         //   else
         //   {
         //       try
         //       {
         //           var result = from r in db.Customers
         //                        where r.Email_Cus == cus.Email_Cus
         //                        select r;
         //           var cus2 = result.ToList().First();
         //           var userSession = new UserLogin();
         //           userSession.UserName = cus2.FirstName;
         //           userSession.UserID = cus2.CodeCus;
         //           Session.Add(Models.Common.CommonConstants.USER_SESSION, userSession);
         //           return RedirectToAction("PurchaseAccount", "ShoppingCart");
         //       }
         //       catch
         //       {
         //           return Content("Error checkout!!!!!");
         //       }

         //   }
        public ActionResult Score(long ? id)
        {
            Customer cus = db.Customers.Find(id);
            if (cus.scores>=0 && cus.scores < 50)
            {
                int diemdong = 50 - Convert.ToInt32(cus.scores);

                int diembac = 100 - Convert.ToInt32(cus.scores);
                int diemvang = 500 - Convert.ToInt32(cus.scores);
                ViewBag.diemdong = diemdong;
                ViewBag.diembac = diembac;
                ViewBag.diemvang = diemvang;
            } 
            else if(cus.scores > 50 && cus.scores < 100)
            {
                int diembac = 100 - Convert.ToInt32(cus.scores);
                int diemvang = 500 - Convert.ToInt32(cus.scores);
                ViewBag.diembac = diembac;
                ViewBag.diemvang = diemvang;
            }
            else
            {
                int diemvang = 500 - Convert.ToInt32(cus.scores);
                ViewBag.diemvang = diemvang;
            }


            return View(cus);
        }

        public ActionResult Bill(long? id)
        {
            var result = from r in db.Orders
                         where r.CodeCus == id
                         select new OrderViewModel()
                         {
                             IDOrder = r.IDOrder,
                             CodeCus = r.CodeCus,
                             OrderDate = r.OrderDate,
                         };
            var ls = result.OrderBy(n => n.OrderDate);
            return View(ls);
        }

        // lich su mua hang
        public ActionResult History()
        {
            var session = (UserLogin)Session[Models.Common.CommonConstants.USER_SESSION];
            if (session == null)
            {
                return View();
            }
            else
            {
                //tk hoa don mua of khach hang
               
                var result = from r in db.Orders
                             where r.CodeCus == session.UserID
                             select new OrderViewModel()
                             {
                                 IDOrder = r.IDOrder,
                                 CodeCus = r.CodeCus,
                                 Status = r.Status,
                                 OrderDate = r.OrderDate,
                             };
                var ls = result.OrderBy(n => n.OrderDate);
                //tim kiem sp trong ctsp
                if (ls == null)
                {
                    return Content("Error!!!");
                }
                else
                {
                    try
                    {
                        List<History> lst = new List<History>();
                        foreach (var item in ls)
                        {
                            var cthdon = db.OrderDetails.Where(n => n.IDOrder == item.IDOrder);
                            var timkiem = from hd in cthdon
                                          join pro in db.Products on hd.IDProduct equals pro.IDProduct into t
                                          from pro in t.DefaultIfEmpty()
                                          select new History
                                          {
                                              IDCus = session.UserID,
                                              OrderDate = item.OrderDate,
                                              ProductName = pro.ProductName,
                                              IDPro = pro.IDProduct,
                                              Quantity = hd.QuantitySale,
                                              Price = pro.Price,
                                              Image = pro.Image,
                                          };
                           

                            foreach ( var i in timkiem)
                            {
                               
                                lst.Add(i); 
                            }
                        }
                        List<History> hs = (lst.GroupBy(i => i.IDPro)
                                             .Select(ss => ss.FirstOrDefault()))
                                            .ToList();
                       
                        return View(hs.ToList());
                    }
                    catch
                    {
                        return Content("Error !!!!!");
                    }
                }
            }
        }
        // chi tiet don hang
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
            if (ddhUpdate != null )
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

        //don hang chua duyet
        public ActionResult NotApproved(long? id)
        {
            var result = from r in db.Orders
                         where r.Status == false && r.Deleted==false && r.CodeCus == id
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
        //huy don hang
        [HttpGet]
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

                ddhUpdate.Deleted =true ;
                ddhUpdate.DeletedDay=DateTime.Now;

                var sumprice = from sp in db.OrderDetails.AsEnumerable()
                               where sp.IDOrder == hdh.IDOrder
                               group sp by sp.IDOrder into g
                               select new
                               {
                                   ids = g.First().IDOrder,
                                   sums = g.Sum(x => x.UnitPriceSale)
                               };
                decimal total = sumprice.FirstOrDefault().sums.Value;

                string content = System.IO.File.ReadAllText(Server.MapPath("~/Static/user/deletedorder.html"));
                content = content.Replace("{{CustomerName}}", ddhUpdate.Customer.FirstName);
                content = content.Replace("{{Phone}}", ddhUpdate.SDT_Cus);
                content = content.Replace("{{Email}}", ddhUpdate.Email_Cus);
                content = content.Replace("{{Address}}", ddhUpdate.Descriptions);
                content = content.Replace("{{Total}}", total.ToString("N0"));
                var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
                new Mail().SendMail(toEmail, "Đơn hàng từ khach hang "+ddhUpdate.Customer.FirstName + " da bi huy", content);
                db.SaveChanges();
                return RedirectToAction("/Bill/" + ddhUpdate.CodeCus);
            }
            else
            {
                ViewBag.Message = "Huy không thành công";
            }
            return View(ddhUpdate);
        }

        //dang giao
        public ActionResult danggiao(long ? id)
        {
            var result = from r in db.Orders
                         where r.Status == true && r.CodeCus == id && r.Deleted == false && r.Paid==false
                         select new OrderViewModel()
                         {
                             IDOrder = r.IDOrder,
                             CodeCus = r.CodeCus,
                             Status = r.Status,
                             OrderDate = r.OrderDate,
                         };
            var ls = result.OrderBy(n => n.OrderDate);
            if(ls==null)
            {
                ViewBag.Message = "Khong co";
            }
            return View(ls);
        }

        public ActionResult danhan(long? id)
        {
            var result = from r in db.Orders
                         where r.Status == true && r.CodeCus == id && r.Deleted == false && r.Paid == true
                         select new OrderViewModel()
                         {
                             IDOrder = r.IDOrder,
                             CodeCus = r.CodeCus,
                             Status = r.Status,
                             OrderDate = r.OrderDate,
                         };
            var ls = result.OrderBy(n => n.OrderDate);
            if (ls == null)
            {
                ViewBag.Message = "Khong co";
            }
            return View(ls);
        }

        //xac nhan nhan don hang
        [HttpGet]
        public ActionResult XacNhan(long? id)
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

        /// <param name="htdh"></param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XacNhan(_Order htdh)
        {
            _Order ddhUpdate = db.Orders.Where(n => n.IDOrder.Equals(htdh.IDOrder)).FirstOrDefault();
            if (ddhUpdate != null)
            {
                ddhUpdate.Paid = true;
                ddhUpdate.Collected = htdh.TotalSuccess;
                db.SaveChanges();
                return RedirectToAction("/Bill/" + ddhUpdate.CodeCus);
            }
            else
            {
                ViewBag.Message = "Xac nhan khong thanh cong";
            }
            return View(ddhUpdate);
        }


        public ActionResult Deleted(long? id)
        {
            var result = from r in db.Orders
                         where r.Deleted == true && r.CodeCus == id
                         select new OrderViewModel()
                         {
                             IDOrder = r.IDOrder,
                             CodeCus = r.CodeCus,
                             OrderDate = r.OrderDate,
                             Deleted = true
                         };
            var ls = result.OrderBy(n => n.OrderDate);
            if (ls == null)
            {
                ViewBag.Message = "Khong co";
            }
            return View(ls);
        }

        //hoan tac don hang
        [HttpGet]
        public ActionResult HoanTac(long? id)
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

        /// <param name="htdh"></param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HoanTac(_Order htdh)
        {
            _Order ddhUpdate = db.Orders.Where(n => n.IDOrder.Equals(htdh.IDOrder)).FirstOrDefault();
            if (ddhUpdate != null)
            {
                var lstorderDetail = db.OrderDetails.Where(n => n.IDOrder == ddhUpdate.IDOrder);

                foreach (var item in lstorderDetail)
                {
                    Product _pro = db.Products.Find(item.IDProduct);
                    _pro.Quantity = _pro.Quantity - item.QuantitySale;
                }
                DateTime dateht = ddhUpdate.OrderDate.Value.AddDays(+1);
                if (ddhUpdate.DeletedDay <= dateht)
                {
                    ddhUpdate.Deleted = false;
                    db.SaveChanges();
                    return RedirectToAction("/Bill/" + ddhUpdate.CodeCus);
                }
                else
                {
                    ViewBag.Message = "Khong the hoan tac do qua thoi han";
                    return RedirectToAction("/Deleted/" + ddhUpdate.CodeCus);
                }
            }
            return View(ddhUpdate);
        }

        public ActionResult EditCuswFace(long? id)
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

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCuswFace(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var customer2 = db.Customers.AsNoTracking().Where(x => x.CodeCus == customer.CodeCus).FirstOrDefault();
                customer.ModifiedDay = DateTime.Now;
                if (customer.Password == null)
                {
                    customer.Password = customer2.Password;
                }
                db.Entry(customer).State = EntityState.Modified;
                if (customer.CreatedDay == null)
                    db.Entry(customer).Property(m => m.CreatedDay).IsModified = false;
                db.SaveChanges();

                return RedirectToAction("/Details/" + customer.CodeCus);
            }
            return View(customer);
        }

    }
}
