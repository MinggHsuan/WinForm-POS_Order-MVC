using POS_Order.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS_Order
{
    public abstract class Discount
    {
        protected List<Item> items;
        protected Discount(List<Item> items)
        {
            this.items = items;
        }
        public abstract void GetResult(List<Item> items);

        public static async Task DisCountOrder(OrderRequest orderRequest)
        {
            orderRequest.items.RemoveAll(x => x.name.Contains("贈送") || x.name.Contains("折扣"));
            DiscountContext discountContext = new DiscountContext(orderRequest);
            (string discountName, string reason) = await discountContext.ApplyStrategy();
            ShowPanel.BuildUp(orderRequest.items, discountName, reason);
        }
    }
}
