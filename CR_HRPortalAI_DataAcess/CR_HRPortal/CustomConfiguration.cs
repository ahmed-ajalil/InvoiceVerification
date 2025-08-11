using System;
using System.Collections.Generic;

namespace CR_HRPortalAI_DataAcess.CR_HRPortal;

public partial class CustomConfiguration
{
    public int Id { get; set; }

    public int? PlanToDevelopAzureAiservice { get; set; }

    public decimal? EstimatePotentialRevenueAzureAi { get; set; }

    public string? OrganizationOperatingMargin { get; set; }

    public decimal? EstimateOperatingAzureMlinvestment { get; set; }

    public string? TotalPeopleWorkAutomatedAzureAi { get; set; }

    public int? TotalPeopleAiMlmodeling { get; set; }

    public int? EstimatevolumeoftasksAnnually { get; set; }

    public decimal? CurrentErrorrateProcesses { get; set; }

    public decimal? AnnualSaving { get; set; }

    public string? OrganizationName { get; set; }

    public decimal? AnnualRevenue { get; set; }

    public bool? BusinessGrowth { get; set; }

    public bool? CostOptimization { get; set; }

    public bool? ManualProcessAutomation { get; set; }

    public bool? AiMlOperationalEfficiency { get; set; }

    public bool? ReplaceLegacyTechnology { get; set; }

    public decimal? EstimateSizeOfRevenueAzureProductandServices { get; set; }
}
