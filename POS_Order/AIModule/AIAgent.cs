using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace POS_Order.AIModule
{
    public class AIAgent
    {
        AIRequest aIRequest = new AIRequest();
        public AIAgent()
        {
            var tools = Assembly.GetExecutingAssembly().DefinedTypes
              .Where(x => x.BaseType == typeof(ToolDeclaration))
              .Select(x =>
              {
                  return (ToolDeclaration)Activator.CreateInstance(Type.GetType(x.FullName));

              }).ToList();

            aIRequest.AddTools(tools);
        }
        public void AddPrompt(AgentType agentType, string text)
        {
            aIRequest.AddPrompt(agentType.ToString(), text);
        }
        public async Task<AIResult> GetResult()
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent");
            request.Headers.Add("x-goog-api-key", "API-KEY");
            string content = JsonConvert.SerializeObject(aIRequest);
            request.Content = new StringContent(content);
            var response = await client.SendAsync(request);

            string responseString = await response.Content.ReadAsStringAsync();
            AIResponse aIResponse = JsonConvert.DeserializeObject<AIResponse>(responseString);
            AIResult result = new AIResult(aIResponse);
            if (!result.CanExcuteTool)
            {
                AddPrompt(AgentType.Model, result.ResponseText);
            }
            result.RunTool();
            AddPrompt(AgentType.Model, result.ResponseText);
            return result;
        }
    }
}
