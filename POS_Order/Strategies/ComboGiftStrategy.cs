using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static POS_Order.MenuModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;

namespace POS_Order.Strategies
{
    public class FindSameItem
    {
        public static List<Conditionbox> FindCommon(List<Item> items, List<Conditionbox> condition)
        {
            var temp = items.Join(condition,
                item => item.name,
                cond => cond.name,
                (item, cond) => new Conditionbox(item.name, item.amount, cond.conditionAmount, cond.conditionID))
                .ToList();
            return temp;
        }
    }
    public class Conditionbox
    {
        public string name; //購買的品項
        public int subtotal;
        public int price;
        public int amount; // 購買數量
        public int conditionAmount; //第幾組的數量條件限制
        public int conditionID; // 第幾組的condition
        public Conditionbox(string name, int price)
        {
            this.name = name;
            this.price = price;
        }
        public Conditionbox(string name, int conditionAmount, int conditionID)
        {
            this.name = name;
            this.conditionAmount = conditionAmount;
            this.conditionID = conditionID;
        }
        public Conditionbox(string name, int amount, int conditionAmount, int conditionID)
        {
            this.name = name;
            this.amount = amount;
            this.conditionAmount = conditionAmount;
            this.conditionID = conditionID;
        }
    }
    public class ComboGiftStrategy : ADiscountStrategy
    {

        public ComboGiftStrategy(MenuModel.Discount discountType, List<Item> items) : base(discountType, items)
        {
        }

        public override void Discount()
        {
            List<Conditionbox> condition = discountType.Conditions
                .SelectMany((x, index) => x.Name.Split('|')
                .Select(name => new Conditionbox(name, x.RequirAmount, index)))
                .ToList();

            List<Conditionbox> buyItem = FindSameItem.FindCommon(items, condition);

            var classify = buyItem.GroupBy(x => new { x.conditionID, x.conditionAmount })
                .Select(x => new
                {
                    totalAmount = x.Sum(y => y.amount),
                    requirAmount = x.Key.conditionAmount
                })
                .Where(x => (x.totalAmount / x.requirAmount) > 0)
                .ToList();

            if (classify.Count != discountType.Conditions.Length)
            {
                return;
            }
            int minCombo = classify.Min(x => x.totalAmount / x.requirAmount);

            List<Conditionbox> RewardsCondition = new List<Conditionbox>();
            Dictionary<int, List<Conditionbox>> RewardsClassify = new Dictionary<int, List<Conditionbox>>();

            RewardsCondition = discountType.Rewards
                .SelectMany((x, index) => x.Name.Split('|')
                .Select(name => new Conditionbox(name, x.RewardsAmount, index)))
                .ToList();

            RewardsClassify = RewardsCondition.GroupBy(x => x.conditionID)
                .ToDictionary(x => x.Key, x => x.ToList());

            var rewards = RewardsClassify.SelectMany(x => x.Value)
                .Join(items,
                reward => reward.name,
                item => item.name,
                (reward, item) => new Conditionbox(item.name, item.price)
                ).ToList();

            items.AddRange(discountType.Rewards.Select(x =>
            {
                Conditionbox gifts = null;
                if (x.RewardType == "")
                {
                    return new Item($"(贈送){x.Name}", 0, minCombo * x.RewardsAmount);
                }
                if (x.RewardType == "Min")
                {
                    int minprice = rewards.Min(y => y.price);
                    gifts = rewards.Where(y => y.price == minprice)
                        .First();
                }
                if (x.RewardType == "Max")
                {
                    int maxprice = rewards.Max(y => y.price);
                    gifts = rewards.Where(y => y.price == maxprice)
                        .First();
                }
                if (x.RewardType == "Random")
                {
                    var rewardnames = x.Name.Split('|');

                    var reward = rewards.Where(y => rewardnames.Contains(y.name)).ToList();
                    gifts = reward.OrderBy(y => new Random(Guid.NewGuid().GetHashCode()).Next()).First();
                }
                return new Item($"(贈送){gifts.name}", 0, minCombo * x.RewardsAmount);
            }));

        }
    }
}
