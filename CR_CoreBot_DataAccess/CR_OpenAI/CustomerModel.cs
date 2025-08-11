using System;
using System.Collections.Generic;

namespace CR_CoreBot_DataAccess.CR_OpenAI;

public partial class CustomerModel
{
    public int Id { get; set; }

    public int? CustomerId { get; set; }

    public string? ModelName { get; set; }

    public string? ModelDisplayName { get; set; }

    public string? ResourceName { get; set; }

    public string? AzureModelName { get; set; }

    public string? ServiceName { get; set; }

    public string? ApiKey { get; set; }
}
