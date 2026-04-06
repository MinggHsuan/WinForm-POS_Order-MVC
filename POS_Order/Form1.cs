using Newtonsoft.Json;
using POS_Order.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
namespace POS_Order
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            menuContainer.AutoGenerate(Checkbox_CheckedChange, Numberic_ValueChange);
            PanelHandlers.Handler += GetPanel;
            comboBox1.DataSource = MenuData.Discounts;
            comboBox1.DisplayMember = "Name";
        }

        private void Checkbox_CheckedChange(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            FlowLayoutPanel panel = (FlowLayoutPanel)checkBox.Parent;
            NumericUpDown numericUpDown = (NumericUpDown)panel.Controls[1];
            numericUpDown.Value = checkBox.Checked == true ? 1 : 0;
        }

        private async void Numberic_ValueChange(object sender, EventArgs e)
        {
            NumericUpDown numericUpDown = (NumericUpDown)sender;
            FlowLayoutPanel panel = (FlowLayoutPanel)numericUpDown.Parent;
            CheckBox checkbox = (CheckBox)panel.Controls[0];
            checkbox.Checked = numericUpDown.Value < 1 ? false : true;
            Item itembox = new Item(checkbox.Text.Split('$')[0],
                int.Parse(checkbox.Text.Split('$')[1]),
                int.Parse(numericUpDown.Value.ToString()));
            OrderRequest orderRequest = new OrderRequest(itembox, (MenuModel.Discount)comboBox1.SelectedValue);
            await Order.AddOrder(orderRequest);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //result = flowLayoutPanel5.AutoClac();
            //totalLab.Text = result.ToString();
            //result = 0;
        }
        private void GetPanel(object sender, RenderData box)
        {
            flowLayoutPanel5.Controls.Clear();
            flowLayoutPanel5.Controls.Add(box.panel);
            totalLab.Text = box.total.ToString();
            RecommandReasonTxt.Text = box.reason;
            int discountIndex = 0;
            for (int i = 0; i < MenuData.Discounts.Length; i++)
            {
                if (MenuData.Discounts[i].Name == box.discountName)
                {
                    discountIndex = i;
                    break;
                }
            }
            comboBox1.SelectedIndex = discountIndex;
            box.total = 0;
        }

        private async void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Enabled && comboBox1.SelectedValue is MenuModel.Discount discountType)
            {
                OrderRequest orderRequest = new OrderRequest(discountType);
                await Order.Checkout(orderRequest);
            }
        }

        private async void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = !checkBox1.Checked;
            OrderRequest orderRequest = new OrderRequest();
            await Order.Checkout(orderRequest);
        }


    }
}

