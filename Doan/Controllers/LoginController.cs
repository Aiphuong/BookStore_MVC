using Doan.Models;

using Doan.Models.Common;
using Doan.Models.MD;
using Doan.Models.ViewModel;
using Facebook;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Doan.Controllers
{
    public class LoginController : Controller
    {

        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }
        // GET: Login
        Db_Doan _db = new Db_Doan();
        // GET: Login


        public ActionResult Index()
        {
            return View();
        }
        //method Login

        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var md = new LoginMD();
                var result = md.Login(model.UserName, GetMD5(model.Password), true);
                if (result == 1)
                {
                    var user = md.GetById(model.UserName);
                    var userSession = new UserLogin();
                    userSession.UserName = user.UserName;
                    userSession.UserID = user.IDUser;
                    userSession.GroupID = user.GroupID;
                    var listCredentials = md.GetListCredential(model.UserName);

                    Session.Add(CommonConstants.SESSION_CREDENTIALS, listCredentials);
                    Session.Add(CommonConstants.USER_SESSION, userSession);
                    return RedirectToAction("Index", "Products");
                }
                else if (result == 4)
                {
                    var user = md.GetById(model.UserName);
                    var userSession = new UserLogin();
                    userSession.UserName = user.UserName;
                    userSession.UserID = user.IDUser;
                    userSession.GroupID = user.GroupID;
                    var listCredentials = md.GetListCredential(model.UserName);

                    Session.Add(CommonConstants.SESSION_CREDENTIALS, listCredentials);
                    Session.Add(CommonConstants.USER_SESSION, userSession);
                    return RedirectToAction("IndexUser", "Home");
                }
                else if (result == 0)
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại.");
                }

                else if (result == -2)
                {
                    ModelState.AddModelError("", "Mật khẩu không đúng.");
                }
                else if (result == -3)
                {
                    ModelState.AddModelError("", "Tài khoản của bạn không có quyền đăng nhập.");
                }
                else
                {
                    ModelState.AddModelError("", "đăng nhập không đúng.");
                }
            }
            return RedirectToAction("Index", "Login");
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Register(User _user)
        {
            if (ModelState.IsValid)
            {
                var check = _db.Users.FirstOrDefault(s => s.Email == _user.Email);
                if (check == null) 
                {
                    _user.Password = GetMD5(_user.Password);
                    _user.GroupID = "MEMBER";
                    _db.Configuration.ValidateOnSaveEnabled = false;
                    _db.Users.Add(_user);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.error = "Email already exist! Use another email!";
                    return View();
                }
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        //create a string MD5
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }


        [HttpGet]
        public ActionResult LoginCus()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginCus(Customer cus)
        {

            var check = _db.Customers.Where(s => s.Email_Cus.Equals(cus.Email_Cus)
            && s.Password.Equals(cus.Password)).FirstOrDefault();
            if (check == null)
            {
                return Content("Error checkout!!!!!");
            }
            else
            {
                try
                {
                    var result = from r in _db.Customers
                                 where r.Email_Cus == cus.Email_Cus
                                 select r;
                    var cus2 = result.ToList().First();
                    var userSession = new UserLogin();
                    userSession.UserName = cus2.FirstName;
                    userSession.UserID = cus2.CodeCus;
                    Session.Add(Models.Common.CommonConstants.USER_SESSION, userSession);
                    return RedirectToAction("IndexUser", "Home");
                }
                catch
                {
                    return Content("Error !!!!!");
                }

            }
        }
        [HttpGet]
        public ActionResult RegisterCus()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult RegisterCus(Customer _user)
        {
            if (ModelState.IsValid)
            {
                var check = _db.Customers.FirstOrDefault(s => s.Email_Cus == _user.Email_Cus);
                if (check == null)
                {
                    //_user.Password = GetMD5(_user.Password);
                    _user.scores = 0;
                    _user.CreatedDay = DateTime.Now;
                    _db.Configuration.ValidateOnSaveEnabled = false;
                    _db.Customers.Add(_user);
                    _db.SaveChanges();
                    return RedirectToAction("LoginCus");
                }
                else
                {
                    ViewBag.error = "Email already exist! Use another email!";
                    return View();
                }
            }
            return View();
        }
        public ActionResult LoginFacebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email",
            });

            return Redirect(loginUrl.AbsoluteUri);
        }

        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });


            var accessToken = result.access_token;
            if (!string.IsNullOrEmpty(accessToken))
            {
                fb.AccessToken = accessToken;
                // Get the user's information, like email, first name, middle name etc
                dynamic me = fb.Get("me?fields=first_name,middle_name,last_name,id,email");
                string email = me.email;
                string userName = me.email;
                string firstname = me.first_name;
                string middlename = me.middle_name;
                string lastname = me.last_name;

                var user = new Customer();
                user.Email_Cus = email;
                user.FirstName = firstname;
                user.LastName = lastname;
                user.CreatedDay = DateTime.Now;

                var check = _db.Customers.SingleOrDefault(x => x.Email_Cus == user.Email_Cus);
                if (check == null)
                {
                    _db.Customers.Add(user);
                    _db.SaveChanges();
                    var userSession = new UserLogin();
                    userSession.UserName = user.FirstName;
                    userSession.UserID = user.CodeCus;
                    Session.Add(Models.Common.CommonConstants.USER_SESSION, userSession);
                    return Redirect("/Customers/EditCuswFace/"+ user.CodeCus);
                }
                else
                {
                    var userSession = new UserLogin();
                    userSession.UserName = check.FirstName;
                    userSession.UserID = check.CodeCus;
                    Session.Add(Models.Common.CommonConstants.USER_SESSION, userSession);
                }
                //var resultInsert = new LoginMD().InsertForFacebook(user);
                //if (resultInsert > 0)
                //{
                //    var userSession = new UserLogin();
                //    userSession.UserName = user.FirstName;
                //    userSession.UserID = user.CodeCus;
                //    Session.Add(Models.Common.CommonConstants.USER_SESSION, userSession);
                //}
            }
            return Redirect("/");
        }
        public ActionResult ResetPass()
        {
            return View();
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPass(Customer cus)
        {
             var check = _db.Customers.AsNoTracking().Where(x => x.Email_Cus == cus.Email_Cus).FirstOrDefault();
                if (check == null)
                {
                    return Content("Tài khoản chưa có trong hệ thống, hãy tạo tài khoản!!!!!");
                }
                else
                {
                    string password = Membership.GeneratePassword(12, 1);
                    check.Password = password;
                //cus.CodeCus = check.CodeCus;
                //cus.Email_Cus = check.Email_Cus;
                //cus.Phone_Cus = check.Phone_Cus;
                //cus.Address_Cus = check.Address_Cus;
                //cus.FirstName = check.FirstName;
                //cus.LastName = check.LastName;
                //cus.CreatedDay = check.CreatedDay;
                //cus.Password = password;
                //_db.Entry(cus).State = EntityState.Modified;
                _db.Entry(check).State = EntityState.Modified;
            }
            _db.SaveChanges();
                string contentt = System.IO.File.ReadAllText(Server.MapPath("~/Static/user/resetpass.html"));
                contentt = contentt.Replace("{{CustomerName}}", check.FirstName);
                contentt = contentt.Replace("{{Email}}", check.Email_Cus);
                contentt = contentt.Replace("{{NewPassword}}", check.Password);

                var toEmails2 = check.Email_Cus;
                new Models.ClassLibrary.Mail().SendMail(toEmails2, "Reset mật khẩu từ Pheonix Bookstore", contentt);

            return RedirectToAction("LoginCus", "Login");

        }

    }

}

