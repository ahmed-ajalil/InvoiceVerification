using System;
using System.Collections.Generic;

namespace CR_HRPortalAI_DataAcess.CR_HRPortal;

public partial class TotalBenefit
{
    public string? Metric { get; set; }

    public decimal? Year1 { get; set; }

    public decimal? Year2 { get; set; }

    public decimal? Year3 { get; set; }

    public decimal? Total { get; set; }

    public decimal? Presentvalue { get; set; }
}
