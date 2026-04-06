using POS_Order.AIModule;
using POS_Order.Discounts;
using POS_Order.Models;
using POS_Order.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_Order
{
    public class DiscountContext
    {
        OrderRequest orderRequest;
        public MenuModel.Discount discountType;
        public List<Item> items;
        Classification classification = null;
        public DiscountContext(OrderRequest orderRequest)
        {
            this.orderRequest = orderRequest;
            this.items = orderRequest.items;
        }

        public async Task<(string, string)> ApplyStrategy()
        {
            string discountName = "";
            string reason = "";
            if (orderRequest.AiRecommand == true)
            {
                string orders = String.Join(",", items.Select(x => $"{x.name} {x.amount}份"));
                AIAgent aIAgent = new AIAgent();
                aIAgent.AddPrompt(AgentType.Model, $"這是當前所有餐點的內容以及折扣方案的json檔案:{MenuData.Menujson}");
                aIAgent.AddPrompt(AgentType.Model, "作為一個AI助理，你需要幫助使用者根據他所點的餐點內容，直接自動幫他決定選擇哪一種折扣的方案最划算，並呼叫Tool觸發");
                aIAgent.AddPrompt(AgentType.User, $"以下這是我所購買的餐點內容: {orders}");

                AIResult aIResult = await aIAgent.GetResult();

                AIResponse.Args args = aIResult.RunTool();
                discountName = args.discount;
                reason = args.reason;
                classification = new Classification(args.discountType, discountName, items);
            }
            else
            {
                this.discountType = orderRequest.Discount;
                this.items = orderRequest.items;
                discountName = orderRequest.Discount.Name;
                classification = new Classification(discountType, items);
            }

            //Type type = Type.GetType(discountType.Strategy);
            //discount = (ADiscountStrategy)Activator.CreateInstance(type, new object[] { discountType, items });
            if (classification.discount != null)
            {
                classification.discount.Discount();
            }
            return (discountName, reason);
        }


    }
}
