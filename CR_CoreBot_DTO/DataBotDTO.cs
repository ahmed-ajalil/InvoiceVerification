using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CR_CoreBot_DTO
{
    public class DataBotDTO
    {
        public string? blobStatus { get; set; }
        public string? DocumentType { get; set; }
        public string? CompletionResult { get; set; }
        public string? SummeryResult { get; set; }
        public string? PropmpInput { get; set; }
        public string? PropmpInputExtract { get; set; }
        public string? Document { get; set; }
        public string? DocumentBot { get; set; }
        public string? diagnosis { get; set; }
        public string? SourceURL { get; set; }
        public string? ActionType { get; set; }
        public List<int>? index { get; set; }
        public List<string>? FileName { get; set; }
        public int FileCount { get; set; }
        public List<string> FilePath { get; set; }
        public dynamic? Result { get; set; }
        public dynamic? Result1 { get; set; }
        public string? FileText { get; set; }

        public string? DataSource { get; set; }
        public string ExtractData { get; set; }
        //
        public string? AudioPath { get; set; }
        public string? SummaryData { get; set; }
        //

        public DateTimeOffset AnalyzeActionsCreatedOn { get; set; }
        public DateTimeOffset? AnalyzeActionsExpiresOn { get; set; }
        public string? AnalyzeActionsId { get; set; }
        public List<string>? AnalyzeActionsError { get; set; }
        public List<string>? AnalyzeActionsMessage { get; set; }
        public List<string>? ExtractSummaryError { get; set; }
        public List<string>? ExtractSummaryMessage { get; set; }
        public int ExtractedSummaryDocumentSentencesCount { get; set; }
        public List<string>? ExtractedSummaryDocumentSentencesText { get; set; }

        public List<string>? Text { get; set; }
        public string? MazorSentiment { get; set; }
        public string? PositiveSentiment { get; set; }
        public string? NegativeSentiment { get; set; }
        public string? NuturalSentiment { get; set; }
        public Dictionary<string, string> SentimentResult { get; set; }
        public string SentimentSentence { get; set; }

        public string TextFilePath { get; set; }
    }
}
