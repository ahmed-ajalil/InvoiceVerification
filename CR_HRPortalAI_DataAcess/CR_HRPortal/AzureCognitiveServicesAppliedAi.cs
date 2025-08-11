using System;
using System.Collections.Generic;

namespace CR_HRPortalAI_DataAcess.CR_HRPortal;

public partial class AzureCognitiveServicesAppliedAi
{
    public int Id { get; set; }

    public int? CustomerId { get; set; }

    public string? ServiceCategory { get; set; }

    public string? ServiceType { get; set; }

    public string? CustomerName { get; set; }

    public string? Region { get; set; }

    public string? Description { get; set; }

    public decimal? EstimatedUpfrontCost { get; set; }

    public string? Refrence { get; set; }

    public bool? Include { get; set; }

    public decimal? EstimatedUpfrontCostCustomised { get; set; }
}
