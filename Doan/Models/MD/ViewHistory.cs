using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doan.Models.MD
{
    public class ViewHistoryItem
    {
        public Product _shopping_product { get; set; }
       
    }
    public class ViewHistory
    {
        List<ViewHistoryItem> items = new List<ViewHistoryItem>();
        public IEnumerable<ViewHistoryItem> Items
        {
            get { return items; }
        }
        public void Add(Product _pro)
        {
            var item = items.FirstOrDefault(s => s._shopping_product.IDProduct == _pro.IDProduct);
            if (item == null)
            {
                items.Add(new ViewHistoryItem
                {
                    _shopping_product = _pro
                });
            }
            else
            {
            }
        }
        public void ClearCart()
        {
            items.Clear();
        }
    }
}