using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CR_CoreBot_DTO
{
    public class PerformanceMatrixCheckerModel
    {
            public int PeformanceId { get; set; }
            public string Username { get; set; }
            public string Prompt { get; set; }
            public string Completion { get; set; }
            public string PromptTokens { get; set; }
            public string CompletionTokens { get; set; }
            public string TotalTokens { get; set; }
            public decimal? Cost { get; set; }
            public decimal? ResponseTime { get; set; }
            public DateTime CurrentDateTime { get; set; }

             public string? IsValid { get; set; }
             public bool? PublicInfo { get; set; }

    }
}
