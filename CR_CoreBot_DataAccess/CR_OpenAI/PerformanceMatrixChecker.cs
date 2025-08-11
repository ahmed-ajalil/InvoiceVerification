using System;
using System.Collections.Generic;

namespace CR_CoreBot_DataAccess.CR_OpenAI;

public partial class PerformanceMatrixChecker
{
    public int PeformanceId { get; set; }

    public string Username { get; set; } = null!;

    public string? Prompt { get; set; }

    public string? Completion { get; set; }

    public string? PromptTokens { get; set; }

    public string? CompletionTokens { get; set; }

    public string? TotalTokens { get; set; }

    public decimal? Cost { get; set; }

    public decimal? ResponseTime { get; set; }

    public DateTime CurrentDateTime { get; set; }

    public string? IsValid { get; set; }

    public bool PublicInfo { get; set; }

    public string? Role { get; set; }

    public string? LoginType { get; set; }

    public int? RoleId { get; set; }
    public bool? Hallucination { get; set; }

    public decimal? HallucinationScore { get; set; }

    public bool? Plagiarism { get; set; }

    public bool? PromptInjection { get; set; }

    public string? HateSeverity { get; set; }

    public string? SelfHarmSeverity { get; set; }

    public string? SexualSeverity { get; set; }

    public string? ViolenceSeverity { get; set; }

    public string? Context { get; set; }
}
