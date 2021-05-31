using Doan.Models;
using Doan.Models.MD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Doan.Controllers
{
    public class WLController : Controller
    {
        // GET: WL
        Db_Doan db = new Db_Doan();
        public WhishList GetWhishList()
        {
            WhishList wl = Session["WhishList"] as WhishList;
            if (wl == null || Session["WhishList"] == null)
            {
                wl = new WhishList();
                Session["WhishList"] = wl;
            }
            return wl;
        }
        public ActionResult AddtoWhishList(long id)
        {
            var pro = db.Products.SingleOrDefault(s => s.IDProduct == id);
            if (pro != null)
            {
                GetWhishList().Add(pro);
            }
            return RedirectToAction("ShowToGetWhishList", "WL");
        }
        public ActionResult ShowToGetWhishList()
        {

            if (Session["WhishList"] == null)
            {
                return RedirectToAction("Empty", "WL");
            }
            WhishList wl = Session["WhishList"] as WhishList;
            return View(wl);
        }
        public PartialViewResult BagWhishList()
        {
            int _t_item = 0;
            WhishList wl = Session["WhishList"] as WhishList;
            if (wl != null)
            {
                _t_item = wl.Total_Quantity();
            }
            ViewBag.infoWhishList = _t_item;
            return PartialView("BagWhishList");
        }

        public ActionResult RemoveWhishList(int id)
        {
            WhishList wl = Session["WhishList"] as WhishList;
            wl.Remove_WhishList_Item(id);
            return RedirectToAction("ShowToGetWhishList", "WL");
        }

        public ActionResult Empty()
        {
            return View();
        }
    }
}