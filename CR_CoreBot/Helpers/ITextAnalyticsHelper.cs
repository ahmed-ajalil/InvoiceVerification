using CR_CoreBot_DataAccess.CR_OpenAI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CR_CoreBot.Helpers
{
    public interface ITextAnalyticsHelper
    {
        Task<DetectLanguageResult> GetDetectedLanguageAsync(string input);
        Task<List<string>> GetEntityAsync(string input, string language = "en");
        Task<List<string>> GetKeyPhrasesAsync(string input, string language = "en");
        Task<SentimentResult> GetTextSentimentAsync(string input, string language = "en");
        Task<string> TextEntityExtraction_Summarize(string input);
        //Task<List<string>> GetEntityAsyncChatGpt(string input);
        Task<string> GetSuggestion(string input);

        Task<List<string>> GetEntityAsyncChatGpt(string input, string prompt, string Industry);

        Task<TblModelConfiguration> getModelConfiguration(string RoleType);
        Task<bool> SaveModelConfiguration(string prompt,string RoleType);
    }
}