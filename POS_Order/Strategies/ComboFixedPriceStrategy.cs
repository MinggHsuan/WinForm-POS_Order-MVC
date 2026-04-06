using POS_Order.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_Order.Strategies
{
    public class ComboFixedPriceStrategy : ADiscountStrategy
    {
        public ComboFixedPriceStrategy(MenuModel.Discount discountType, List<Item> items) : base(discountType, items)
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

            var setPrice = MenuData.Menus.SelectMany(x => x.Foods)
                .Join(buyItem,
                menu => menu.Name,
                buy => buy.name,
                (menu, buy) => new
                {
                    name = menu.Name,
                    price = menu.Price,
                    buy.amount,
                    buy.conditionAmount,
                    buy.conditionID,
                    subtotal = menu.Price * buy.conditionAmount,
                }).ToList();


            var setBox = setPrice.GroupBy(x => new { x.conditionID, x.conditionAmount })
                                 .Select(x =>
                                    x.SelectMany(y => Enumerable.Repeat(y.price, y.amount)
                                     .OrderBy(z => z))
                                     .Take(x.Key.conditionAmount * minCombo)
                                     .Sum()
                                 ).Sum();


            items.AddRange(discountType.Rewards.Select(x =>
            {
                int discountPrice = x.RewardsSetPrice;
                if (x.RewardsOff != 0)
                {
                    discountPrice = (int)(float)(setPrice.Sum(y => y.subtotal) * (1 - x.RewardsOff));
                    return new Item($"(折扣)", -discountPrice * minCombo, 1);
                }
                if (x.RewardsPrice != 0)
                {
                    return new Item($"(折扣)", x.RewardsPrice * minCombo - setBox, 1);
                }
                return new Item($"(折扣)", discountPrice * minCombo - setBox, 1);
                //return new Item($"(折扣)", (discountPrice - setPrice.Sum(y => y.subtotal)) * minCombo, 1);
            }));
        }
    }
}
