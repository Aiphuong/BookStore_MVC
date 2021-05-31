using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doan.Models.MD
{
    public class BestSaleMD:BestSale
    {
        public IEnumerable<BestSale> GetBestSale() //Tiếp xúc với bộ liệt kê, hỗ trợ một phép lặp đơn giản trên một bộ sưu tập của một loại được chỉ định.
        {
            Db_Doan db = new Db_Doan();
            var bestsale = from bs in db.OrderDetails.AsEnumerable()
                           group bs by bs.IDProduct into g
                           orderby g.Sum(x => x.IDProduct) descending
                           select new BestSale
                           {
                               IDPro = g.Key,
                               Image=g.First().Product.Image,
                               NamePro = g.First().Product.ProductName,
                               Price = g.First().Product.Price,
                               Quantity = g.Sum(x => x.QuantitySale),
                               QuantitySucess = g.First().QuantitySale
                           };
            List<BestSale> h = new List<BestSale>();
            foreach (var item in bestsale)
            {
                var result = from r in db.Products
                             where r.IDProduct == item.IDPro
                             select r;
                foreach (var item2 in result)
                {
                    item.QuantitySucess = item2.Quantity;
                    h.Add(item);
                   
                }
            }
           
            return h.Take(9).ToList();
        }
    }
}