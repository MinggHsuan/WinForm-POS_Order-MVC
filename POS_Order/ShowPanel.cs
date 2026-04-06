using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS_Order
{
    public class ShowPanel
    {
        public static void BuildUp(List<Item> items, string discountName, string reason)
        {
            int total = 0;
            FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
            flowLayoutPanel.Width = 444;
            flowLayoutPanel.Height = 523;
            Label itemname = new Label();
            itemname.Text = "品項";
            itemname.Width = 60;
            Label pricename = new Label();
            pricename.Text = "價格";
            pricename.Width = 50;
            Label amountname = new Label();
            amountname.Text = "數量";
            amountname.Width = 50;
            Label subtotalname = new Label();
            subtotalname.Text = "小計";
            subtotalname.Width = 50;
            flowLayoutPanel.Controls.Add(itemname);
            flowLayoutPanel.Controls.Add(pricename);
            flowLayoutPanel.Controls.Add(amountname);
            flowLayoutPanel.Controls.Add(subtotalname);

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].amount == 0)
                {
                    continue;
                }
                FlowLayoutPanel panelbox = new FlowLayoutPanel();
                panelbox.Width = 250;
                panelbox.Height = 20;
                flowLayoutPanel.Controls.Add(panelbox);

                Label item = new Label();
                item.Text = items[i].name;
                item.Width = 60;

                Label price = new Label();
                price.Text = items[i].price + "元";
                price.Width = 50;

                Label amount = new Label();
                amount.Text = items[i].amount + "個";
                amount.Width = 50;

                Label subtotal = new Label();
                subtotal.Text = items[i].subtotal + "元";
                subtotal.Width = 55;

                total += items[i].subtotal;

                panelbox.Controls.Add(item);
                panelbox.Controls.Add(price);
                panelbox.Controls.Add(amount);
                panelbox.Controls.Add(subtotal);
            }
            RenderData renderData = new RenderData(flowLayoutPanel, total, discountName, reason);
            PanelHandlers.Notify(renderData);

        }

    }
}
