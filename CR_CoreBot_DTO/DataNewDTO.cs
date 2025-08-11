using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CR_CoreBot_DTO
{
    public class DataNewDTO
    {
        public string? CompletionResult { get; set; }
        public string? StructuredData_Context { get; set; }
        public string? SummeryResult { get; set; }
        public string? PropmpInput { get; set; }
        public string? Document { get; set; }
        public string? SuggestedPrompts { get; set; }
        public List<string>? SourceURL { get; set; }
        public string? IsMarked { get; set; }
        public string? MainPrompt { get; set; }
        public string? Spammer { get; set; }
        public List<string>? LabelName { get; set; }
        public string? messageSuggestion1 { get; set; }
        public string? messageSuggestion2 { get; set; }
        public string? OutScope { get; set; }
        public string? blobStatus { get; set; }
        public string? HyperInput { get; set; }
        public double? ResponseTime { get; set; }
        public DateTime? CurrentTime { get; set; }
        public decimal Cost { get; set; }
        public string PromptTokens { get; set; }
        public string CompletionTokens { get; set; }
        public string TotalTokens { get; set; }
        public bool? PublicInfo { get; set; }
        public string? DocumentType { get; set; }
        public string? GeneratedTextOfAudio { get; set; }
        public string? Sentiments { get; set; }
        public string? newSuggestions1 { get; set; }
        public string? newSuggestions2 { get; set; }
        public string? newSuggestions3 { get; set; }
        public string Suggestions { get; set; }
        public bool personalbanking { get; set; }
		public string presuggestions1 { get; set; }
		public string presuggestions2 { get; set; }
		public string presuggestions3 { get; set; }
        public bool ContentSaftey { get; set; }
        public string DynamicDataResult { get; set; }
        public int CacheStatus { get; set; }
        public ContentSafetyDTO ResponsibleAIAssessment { get; set; }
        public List<SourceDocument> source_document { get; set; }
        //public List<string> source_document { get; set; }

    }
}
