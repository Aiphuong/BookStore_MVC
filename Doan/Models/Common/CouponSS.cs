using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doan.Models.Common
{
    //public class Coupon
    //{
    //}
    [Serializable]
    public class CouponSS
    {
        public string Name { set; get; }
        public decimal Price { set; get; }
    }
}