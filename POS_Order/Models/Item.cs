using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS_Order
{
    public class Item
    {

        public string name;
        public int price;
        public int amount;
        public int subtotal;
        public Item(string name, int price, int amount)
        {
            this.name = name;
            this.price = price;
            this.amount = amount;
            this.subtotal = price * amount;
        }
    }
}
