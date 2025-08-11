using Azure.AI.OpenAI;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace CR_CoreBot_Service.Adapter
{
    public class AzureOpenAI
    {
        public static string fnAzureOpenAI(string prompt, string key, string ModelName, string endpoint)
        {
            try
            {
                OpenAIClient client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));
                var chatCompletionsOptions = new ChatCompletionsOptions()
                {
                    Messages = { new ChatMessage(ChatRole.User, prompt.Trim()) },
                    MaxTokens = 500,
                    Temperature = Convert.ToSingle(0.7),
                };
                Response<ChatCompletions> response = client.GetChatCompletions(
                    deploymentOrModelName: ModelName,
                    chatCompletionsOptions);
                return response.Value.Choices[0].Message.Content;
            }
            catch (Exception ex)
            {
                return " " + ex.Message;
            }
        }
    }
}
