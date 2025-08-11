using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CR_CoreBot_DataAccess.Models
{
    public class ContentSafetyModel
    {
        public int totPromptInj { get; set; }
        public int totPromptInjPass { get; set; }
        public int totPromptInjFail { get; set; }
        public decimal? totPromptInjScore { get; set; }
        public int totPromptHal { get; set; }
        public int totPromptHalPass { get; set; }
        public int totPromptHalFail { get; set; }
        public decimal? totPromptHalScore { get; set; }
        public int totPromptPlg { get; set; }
        public int totPromptPlgPass { get; set; }
        public int totPromptPlgFail { get; set; }
        public decimal? totPromptPlgScore { get; set; }
        public int totPromptTx { get; set; }
        public decimal? totPromptTxHtSc { get; set; }
        public decimal? totPromptTxShSc { get; set; }
        public decimal? totPromptTxSsSc { get; set; }
        public decimal? totPromptTxVlSC { get; set; }
        public int totPromptImg { get; set; }
        public decimal? totPromptImgHtSc { get; set; }
        public decimal? totPromptImgShSc { get; set; }
        public decimal? totPromptImgSsSc { get; set; }
        public decimal? totPromptImgVlSC { get; set; }
        public List<promptInjectionDatasetDetail> promptinjectiondataset { get; set; }
        public List<promptInjectionDModelDetail> promptinjectiondmodel { get; set; }
        public List<prompt> promptModel;
        public List<Toxicityprompt> ToxicityPromptModel;
        public List<ImageToxicity> ImageToxicityModel;
        public List<evaluationtypeall> evaluation;
        public List<LLMAssessmentDetailData> LLMAssessmentDetail { get; set; }
        public List<LLMAssessmentData> LLMAssessment { get; set; }
        public List<ChatbotFileName>? ChatbotFileName { get; set; }
        public List<PromptAssessments>? promptAssessments { get; set; }
    }
    public class ProtectedMaterialAnalysisDTO
    {
        public bool Detected { get; set; }
    }
    public class PlagiarismResponse
    {
        public ProtectedMaterialAnalysisDTO? ProtectedMaterialAnalysis { get; set; }
    }
    public class PromptInjectionResponse
    {
        public UserPromptAnalysis? UserPromptAnalysis { get; set; }
        //public List<DocumentAnalysis>? DocumentsAnalysis { get; set; }
    }
    public class UserPromptAnalysis
    {
        public bool AttackDetected { get; set; }
    }
    public class HallucinationResponseDTO
    {
        public bool? ungroundedDetected { get; set; }
        public int? ungroundedPercentage { get; set; }
        public List<UngroundedDetail>? ungroundedDetails { get; set; }
    }
    public class UngroundedDetail
    {
        public string text { get; set; }
        public Offset offset { get; set; }
        public Length length { get; set; }
        public string reason { get; set; }
    }
    public class Offset
    {
        public int utf8 { get; set; }
        public int utf16 { get; set; }
        public int codePoint { get; set; }
    }
    public class Length
    {
        public int utf8 { get; set; }
        public int utf16 { get; set; }
        public int codePoint { get; set; }
    }
    public class ChatbotFileName
    {
        public string? ModelID { get; set; }
        public string? FileName { get; set; }
        public string? Evaluation { get; set; }
    }
    public class evaluationtypeall
    {
        public string Evaluation { get; set; }
        public string Category { get; set; }
    }
    public class LLMAssessmentDetailData
    {
        public int Id { get; set; }
        public string Evaluation { get; set; }
        public string EvaluationEncr { get; set; }
        public DateTime EvaluationTime { get; set; }
        public string Category { get; set; }
        public int Status { get; set; }
    }
    public class LLMAssessmentData
    {
        public int Id { get; set; }
        public string Evaluation { get; set; }
        public DateTime EvaluationTime { get; set; }
        public string Category { get; set; }
        public int Status { get; set; }
    }
    public class assessModel
    {
        public string Type { get; set; }
        public int TotalPrompt { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public decimal? Score { get; set; }
        public decimal? HateScore { get; set; }
        public decimal? SelfHarmScore { get; set; }
        public decimal? SexualScore { get; set; }
        public decimal? ViolenceScore { get; set; }
    }
    public class promptInjectionDatasetDetail
    {
        public int DSID { get; set; }
        public string DatasetName { get; set; }
        public string DatasetSize { get; set; }
        public bool IsMultiling { get; set; }
        public string MLName { get; set; }
        public int PromotNSize { get; set; }
        public int PromptMSize { get; set; }
        public string TypeOfAssessmnet { get; set; }
        public string LibraryName { get; set; }
    }
    public class promptInjectionDModelDetail
    {
        public int MID { get; set; }
        public string ModelName { get; set; }
        public string Type { get; set; }
        public string Method { get; set; }
    }
    public class AvailableChatBots
    {
        public string Category { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ChatBotId { get; set; }
        public string Language { get; set; }
        public bool Enable { get; set; }
    }
    public class prompt
    {
        public int Id { get; set; }
        public string Prompt { get; set; }
        public string Response { get; set; }
        public string Type { get; set; }
        public bool Result { get; set; }
        public string Recommendation { get; set; }
        public string ReferenceUrl { get; set; }
    }
    public class ImageToxicity
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public string HateSeverity { get; set; }
        public string SelfHarmSeverity { get; set; }
        public string SexualSeverity { get; set; }
        public string ViolenceSeverity { get; set; }
        public string Recommendation { get; set; }
        public string ReferenceUrl { get; set; }

    }
    public class Toxicityprompt
    {
        public int Id { get; set; }
        public string? Prompt { get; set; }
        public string? ResponseText { get; set; }
        public string? HateSeverity { get; set; }
        public string? SelfHarmSeverity { get; set; }
        public string? SexualSeverity { get; set; }
        public string? ViolenceSeverity { get; set; }
        public string? Recommendation { get; set; }
        public string? ReferenceUrl { get; set; }
    }
    public class PromptAssessments
    {
        public string Assessment { get; set; }
        public bool Enable { get; set; }
    }
}
