using System;
using System.Collections.Generic;

namespace CR_CoreBot_DataAccess.CR_OpenAI;

public partial class AzureMlCompute
{
    public int Id { get; set; }

    public int? CustomerId { get; set; }

    public string? ServiceCategory { get; set; }

    public string? ServiceType { get; set; }

    public string? CustomerName { get; set; }

    public string? Region { get; set; }

    public string? Description { get; set; }

    public string? EstimatedMonthlyCost { get; set; }

    public string? EstimatedUpfrontCost { get; set; }

    public string? Refrence { get; set; }
}
