using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_Order.AIModule
{
    public class AIRequest
    {
        public List<Content> contents { get; set; }
        public List<Tool> tools { get; set; } = new List<AIRequest.Tool>() { new AIRequest.Tool() };

        public AIRequest()
        {
            this.contents = new List<AIRequest.Content>();
        }

        public void AddPrompt(string role, string text)
        {
            contents.Add(new Content(role, text));
        }
        public void AddTool(ToolDeclaration toolDeclaration)
        {
            tools[0].functionDeclarations.Add(toolDeclaration);
        }

        public void AddTools(List<ToolDeclaration> toolDeclaration)
        {
            tools[0].functionDeclarations.AddRange(toolDeclaration);
        }
        public class Content
        {
            public string role { get; set; }
            public List<Part> parts { get; set; } = new List<Part>();
            public Content(string role, string text)
            {
                this.role = role;
                parts.Add(new AIRequest.Part(text));
            }

        }

        public class Part
        {
            public object text { get; set; }
            public Part(object text)
            {
                this.text = text;
            }
        }

        public class Tool
        {
            public List<ToolDeclaration> functionDeclarations { get; set; } = new List<ToolDeclaration>();
        }
        public class Items
        {
            public string type { get; set; }
        }
        public class PropertyDetail
        {
            public string type { get; set; }
            public string description { get; set; }
            public Items items { get; set; }
            public List<string> @enum { get; set; }
        }
    }
}
