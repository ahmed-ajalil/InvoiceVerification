using Microsoft.Rest;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Azure.AI.OpenAI;
using Azure;
using CR_CoreBot_DataAccess.CR_OpenAI;
using Microsoft.EntityFrameworkCore;
using CR_CoreBot.Models;
using Microsoft.Extensions.Options;
using CR_CoreBot_DTO;

namespace CR_CoreBot.Helpers
{
    public class TextAnalyticsHelper : ITextAnalyticsHelper
    {
        private static AppSettingsDTO _appSettings;
        private static ITextAnalyticsClient AnalyticsClient { get; set; }
        public TextAnalyticsHelper(IOptions<AppSettingsDTO> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        private class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                string ApiKey = _appSettings.AzureCognitiveKey;
                request.Headers.Add("Ocp-Apim-Subscription-Key", ApiKey);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }
        public async Task<DetectLanguageResult> GetDetectedLanguageAsync(string? input)
        {
            try
            {
                DetectLanguageResult languageResult = new DetectLanguageResult() { Language = new Dictionary<string, string>() };
                if (!string.IsNullOrEmpty(input))
                {
                    AnalyticsClient = new TextAnalyticsClient(new ApiKeyServiceClientCredentials())
                    {
                        Endpoint = _appSettings.AzureCognitiveEndpoint
                    };

                    LanguageBatchInput batchInput = new LanguageBatchInput(new List<LanguageInput>{
                        new LanguageInput("US","0", input)
                    });
                    LanguageBatchResult result = await AnalyticsClient.DetectLanguageAsync(true, batchInput);

                    if (result.Documents != null)
                    {
                        languageResult.Language.Add("iso6391Name", result.Documents[0].DetectedLanguages[0].Iso6391Name);
                        languageResult.Language.Add("name", result.Documents[0].DetectedLanguages[0].Name);
                        languageResult.Language.Add("score", result.Documents[0].DetectedLanguages[0].Score.ToString());
                    }

                    if (result.Errors != null)
                    {
                        // Just return the empty Dictionary
                    }
                }

                return languageResult;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<SentimentResult> GetTextSentimentAsync(string input, string language = "en")
        {
            SentimentResult sentimentResult = new SentimentResult() { Score = 0.5 };

            if (!string.IsNullOrEmpty(input))
            {
                AnalyticsClient = new TextAnalyticsClient(new ApiKeyServiceClientCredentials())
                {
                    Endpoint = _appSettings.AzureCognitiveEndpoint
                };

                SentimentBatchResult result = await AnalyticsClient.SentimentAsync(true, new MultiLanguageBatchInput(
                    new List<MultiLanguageInput>()
                    {
                        new MultiLanguageInput(language, "0", input)
                    }));

                if (result.Documents != null)
                {
                    sentimentResult.Score = (double)result.Documents[0].Score;
                }

                if (result.Errors != null)
                {
                    // Just return the neutral value
                }
            }

            return sentimentResult;
        }
        public async Task<List<string>> GetKeyPhrasesAsync(string input, string language = "en")
        {
            KeyPhrasesResult keyPhrasesResult = new KeyPhrasesResult() { KeyPhrases = Enumerable.Empty<string>() };
            List<string> phrases = new List<string>();
            if (!string.IsNullOrEmpty(input))
            {
                AnalyticsClient = new TextAnalyticsClient(new ApiKeyServiceClientCredentials())
                {
                    Endpoint = _appSettings.AzureCognitiveEndpoint
                };
                KeyPhraseBatchResult result = await AnalyticsClient.KeyPhrasesAsync(true, new MultiLanguageBatchInput(
                    new List<MultiLanguageInput>()
                    {
                        new MultiLanguageInput(language, "0", input)
                    }));

                if (result.Documents != null)
                {


                    foreach (string keyPhrase in result.Documents[0].KeyPhrases)
                    {
                        phrases.Add(keyPhrase);
                    }
                }

                if (result.Errors != null)
                {
                    // Just return the empty IEnumerable
                }
            }

            return phrases;
        }
        public async Task<List<string>> GetEntityAsync(string input, string language = "en")
        {
            List<string> entities = new List<string>();
            EntitiesBatchResult entityResult = await AnalyticsClient.EntitiesAsync(true, new MultiLanguageBatchInput(
                    new List<MultiLanguageInput>()
                    {
                        new MultiLanguageInput(language, "0", input)
                    }));
            if (entityResult.Documents != null)
            {
                foreach (EntityRecord entity in entityResult.Documents[0].Entities)
                {
                    entities.Add(entity.Name);
                }
            }
            return entities;
        }
        public async Task<string> TextEntityExtraction_Summarize(string input)
        {
            try
            {
                string SummeryResult = "";
                string document = string.Empty;
                string key = _appSettings.Key;
                // string ModelName = "gpt-35-turbo";
                string ModelName = _appSettings.ModelName;
                if (!string.IsNullOrEmpty(input))
                {
                    SummeryResult = fnAzureOpenAI(input, "Provide a summary", key, ModelName);
                }
                return SummeryResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return "";
        }
        private static string fnAzureOpenAI(string document, string prompt, string key, string ModelName)
        {
            try
            {
                string endpoint = _appSettings.Endpoint;
                OpenAIClient client = new(new Uri(endpoint), new AzureKeyCredential(key));
                var chatCompletionsOptions = new ChatCompletionsOptions()
                {
                    Messages = { new ChatMessage(ChatRole.System, document.Trim()), new Azure.AI.OpenAI.ChatMessage(ChatRole.User, prompt.Trim()), },
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
        private static string fnAzureOpenAIRole(string document, string prompt, string prompt1, string key, string ModelName)
        {
            try
            {
                string endpoint = _appSettings.Endpoint;
                OpenAIClient client = new(new Uri(endpoint), new AzureKeyCredential(key));
                var chatCompletionsOptions = new ChatCompletionsOptions()
                {
                    Messages ={ new ChatMessage(ChatRole.System,prompt),new ChatMessage(ChatRole.System, prompt1),
                        new ChatMessage(ChatRole.User, document),
                        },
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

        public async Task<string> GetSuggestion(string input)
        {
            try
            {
                string suggestion = "";
                string document = string.Empty;
                string key = _appSettings.Key;
                string ModelName = _appSettings.ModelName;
                if (!string.IsNullOrEmpty(input))
                {
                    suggestion = fnAzureOpenAI(input, "Being an AI model that enhances agent performance by providing real-time, context-aware suggestions and information during customer interactions in the customer support domain and give me the response in points.", key, ModelName);
                }
                return suggestion;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return "";

        }
        public async Task<List<string>> GetEntityAsyncChatGpt(string input, string prompt, string Industry)
        {
            List<string> entities = new List<string>();
            try
            {
                string prompt1 = "Extract " + Industry + " realted entity entities from \r\nbelow text. Extract only the above mentioned ";

                string document = string.Empty;
                string key = _appSettings.Key;
                string ModelName = _appSettings.ModelName;
                if (!string.IsNullOrEmpty(input))
                {
                    List<string> splitSummary = new List<string>();
                    var entitysummary = fnAzureOpenAIRole(input, prompt, prompt1, key, ModelName);
                    if (entitysummary.Contains("\n"))
                    {
                        splitSummary = entitysummary.Split("\n").ToList();
                        for (int i = 1; i < splitSummary.Count; i++)
                        {
                            entities.Add(splitSummary[i].Replace("-", " ").Replace('"', ' ').Replace("]", " ").Replace("'", " "));
                        }
                    }
                    else if (entitysummary.Contains(","))
                    {
                        splitSummary = entitysummary.Split(",").ToList();
                        for (int i = 1; i < splitSummary.Count; i++)
                        {
                            entities.Add(splitSummary[i].Replace("-", " ").Replace('"', ' ').Replace("]", " ").Replace("'", " "));
                        }
                    }
                    else
                    {
                        entities.Add(entitysummary);
                    }

                }
            }
            catch (Exception ex)
            {
                entities.Add(ex.Message.ToString());
            }
            return entities;
        }


        public async Task<TblModelConfiguration> getModelConfiguration(string RoleType)
        {
            TblModelConfiguration tblModelConfigurations = new TblModelConfiguration();
            try
            {
                CropenAiContext context = new CropenAiContext();
                tblModelConfigurations = context.TblModelConfigurations
                                .Where(x => x.Industry == RoleType).FirstOrDefault();
            }
            catch (Exception ex)
            {

            }
            return tblModelConfigurations;
        }
        public async Task<bool> SaveModelConfiguration(string prompt,string RoleType)
        {
            bool _status = false;
            try
            {
                using (var context = new CropenAiContext())
                {
                    var bankingConfigurations = context.TblModelConfigurations
                        .Where(config => config.Industry == RoleType).FirstOrDefault();
                    if(bankingConfigurations.ConfigurationText != prompt)
                    {
                        bankingConfigurations.ConfigurationText = prompt;
                        bankingConfigurations.CreateDate = DateTime.Now;
                        await context.SaveChangesAsync();
                    }
                }
                _status = true;
            }
            catch (Exception ex)
            {
                _status = false;
            }
            return _status;
        }



    }
    public class DetectLanguageResult
    {
        public Dictionary<string, string> Language { get; set; }
    }
    public class SentimentResult
    {
        public double Score { get; set; }
    }
    public class KeyPhrasesResult
    {
        public IEnumerable<string> KeyPhrases { get; set; }
    }
    public class TranslateTextResult
    {
        public string TranslatedText { get; set; }
    }
}
