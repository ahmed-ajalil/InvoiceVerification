using CR_CoreBot_DataAccess.CR_OpenAI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CR_CoreBot_DTO
{
    public class AiAssessmentDTO
    {
        public List<assessmentScore>? aiAssessment { get; set; }
        public List<PerformanceMatrixChecker> performanceMatrixCheckers { get; set; }
    }
    //public class PerformanceMatrixChecker
    //{
    //    [Key]
    //    public int PeformanceId { get; set; }

    //    public string Username { get; set; } = null!;

    //    public string? Prompt { get; set; }

    //    public string? Completion { get; set; }

    //    public string? PromptTokens { get; set; }

    //    public string? CompletionTokens { get; set; }

    //    public string? TotalTokens { get; set; }

    //    public decimal? Cost { get; set; }

    //    public decimal? ResponseTime { get; set; }

    //    public DateTime CurrentDateTime { get; set; }

    //    public string? IsValid { get; set; }

    //    public bool PublicInfo { get; set; }

    //    public string? Role { get; set; }

    //    public string? LoginType { get; set; }

    //    public int? RoleId { get; set; }

    //    public bool? Hallucination { get; set; }

    //    public decimal? HallucinationScore { get; set; }

    //    public bool? Plagiarism { get; set; }

    //    public bool? PromptInjection { get; set; }

    //    public string? HateSeverity { get; set; }

    //    public string? SelfHarmSeverity { get; set; }

    //    public string? SexualSeverity { get; set; }

    //    public string? ViolenceSeverity { get; set; }

    //    public string? Context { get; set; }
    //}
    public class assessmentScore
    {
        public string type { get; set; }
        public double? score { get; set; }
    }
}
