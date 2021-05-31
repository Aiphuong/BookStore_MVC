using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doan.Models.MD
{
    public class WhishListItem
    {
        public Product _shopping_product { get; set; }
        public int _shopping_quantity { get; set; }
    }
    public class WhishList
    {
        List<WhishListItem> items = new List<WhishListItem>();
        public IEnumerable<WhishListItem> Items
        {
            get { return items; }
        }
        public void Add(Product _pro, int _quantity = 1)
        {
            var item = items.FirstOrDefault(s => s._shopping_product.IDProduct == _pro.IDProduct);
            if (item == null)
            {
                items.Add(new WhishListItem
                {
                    _shopping_product = _pro,
                    _shopping_quantity = _quantity
                });
            }
            else
            {
                item._shopping_quantity += _quantity;
            }
        }

        public void Remove_WhishList_Item(int id)
        {
            items.RemoveAll(s => s._shopping_product.IDProduct == id);
        }
        public int Total_Quantity()
        {
            return items.Sum(s => s._shopping_quantity);
        }
    }
}