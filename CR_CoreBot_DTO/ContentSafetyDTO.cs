using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CR_CoreBot_DTO
{
    public class ContentSafetyDTO
    {
        public List<assessDTO> assessdto { get; set; }
        public assessDTO? ToxicityRes { get; set; }
        public HallucinationResponseCS? hallucinationResponse { get; set; }
        public PromptInjectionResponseCS? PromptInjectionRes { get; set; }
        public PlagiarismResponseCS? PlagiarismRes { get; set; }

        public List<evaluationtypeall> evaluation;
        public bool? isSafe { get; set; }
        public bool groundednessAvailablity { get; set; }
        public bool plagraismAvailablity { get; set; }
        public bool promptInjectionAvailablity { get; set; }
        public bool toxicityAvailablity { get; set; }
        public Exception? exception { get; set; }
    }

    public class HallucinationResponseCS
    {
        public bool? ungroundedDetected { get; set; }
        public float? ungroundedPercentage { get; set; }
        public List<UngroundedDetail>? ungroundedDetails { get; set; }
    }
    public class UngroundedDetail
    {
        public string? text { get; set; }
        public Offset? offset { get; set; }
        public Length? length { get; set; }
        public string? reason { get; set; }
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
    public class PromptInjectionResponseCS
    {
        public UserPromptAnalysis? UserPromptAnalysis { get; set; }
        public List<DocumentAnalysis>? DocumentsAnalysis { get; set; }
    }
    public class UserPromptAnalysis
    {
        public bool AttackDetected { get; set; }
    }

    public class DocumentAnalysis
    {
        public bool AttackDetected { get; set; }
    }
    public class PlagiarismResponseCS
    {
        public ProtectedMaterialAnalysisDTO? ProtectedMaterialAnalysis { get; set; }
    }
    public class ProtectedMaterialAnalysisDTO
    {
        public bool Detected { get; set; }
    }

    public class assessDTO
    {
        public string Type { get; set; }
        public int TotalPrompt { get; set; }
        public int Passed { get; set; }
        public int Failed { get; }
        public decimal? Score { get; set; }
        public decimal? HateScore { get; set; }
        public decimal? SelfHarmScore { get; set; }
        public decimal? SexualScore { get; set; }
        public decimal? ViolenceScore { get; set; }
    }
    public class evaluationtypeall
    {
        public string Evaluation { get; set; }
        public string Category { get; set; }
    }
    public class ContentFilterResult
    {
        public bool Filtered { get; set; }
        public string Severity { get; set; }
    }
    public class ErrorDetails
    {
        public ContentFilterResult Hate { get; set; }
        public ContentFilterResult Self_Harm { get; set; }
        public ContentFilterResult Sexual { get; set; }
        public ContentFilterResult Violence { get; set; }
    }
}
