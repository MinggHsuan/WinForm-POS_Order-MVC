using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static POS_Order.AIModule.AIRequest;

namespace POS_Order.AIModule.DiscountTool
{
    internal class DiscountParameter : Parameters
    {
        public override object properties => new
        {
            discountType = new PropertyDetail()
            {
                type = "string",
                description = "這是折扣名稱的路徑，是一個字串，例如:POS_Order.Strategies.ItemPercentageDiscountStrategy"
            },
            discount = new PropertyDetail()
            {
                type = "string",
                description = "這是折扣的名稱，是一個字串，例如:雞排飯買三個打85折"
            },
            reason = new PropertyDetail()
            {
                type = "string",
                description = "這是選擇此折扣的原因是什麼，是一個字串"
            }
        };
        public override string[] required => new string[] { "discountType", "discount", "reason" };
    }
}
