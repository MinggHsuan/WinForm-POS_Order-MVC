using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_Order.AIModule.DiscountTool
{
    internal class UseDiscountTool : ToolsFunction
    {
        public UseDiscountTool(AIResponse.Args args) : base(args)
        {
        }
        public override AIResponse.Args UseTools()
        {
            return args;
        }
    }
}
