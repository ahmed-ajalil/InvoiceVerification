using System;
using System.Collections.Generic;

namespace CR_CoreBot_DataAccess.CR_OpenAI;

public partial class BotConfiguration
{
    public int CustomerId { get; set; }

    public string? OpenaiApiVersion { get; set; }

    public decimal? Temperature { get; set; }

    public int? MaxTokens { get; set; }

    public string? AzureEndpoint { get; set; }

    public string? OpenaiApiKey { get; set; }

    public string? OpenaiApiType { get; set; }

    public string? DeploymentName { get; set; }
}
