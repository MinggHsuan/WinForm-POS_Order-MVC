using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static POS_Order.AIModule.AIResponse;

namespace POS_Order.AIModule
{
    public class AIResult
    {
        AIResponse response;
        public string ResponseText;
        public string ResponseSource;
        public string reason;
        public bool CanExcuteTool;
        public AIResult(AIResponse response)
        {
            this.response = response;
            this.ResponseText = response.candidates[0].content.parts[0].functionCall.args.reason;
            this.ResponseSource = response.candidates[0].content.parts[0].functionCall.args.discountType;
            this.CanExcuteTool = response.candidates[0].content.parts[0].functionCall != null;
        }

        public AIResponse.Args RunTool()
        {
            var toolCall = response.candidates[0].content.parts[0].functionCall;
            Type type = Type.GetType("POS_Order." + toolCall.name);
            var tool = (ToolsFunction)Activator.CreateInstance(type, new object[] { toolCall.args });
            return tool.UseTools();
        }
    }
}
