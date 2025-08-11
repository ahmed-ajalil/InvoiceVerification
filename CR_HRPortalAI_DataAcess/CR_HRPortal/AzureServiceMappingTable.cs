using System;
using System.Collections.Generic;

namespace CR_HRPortalAI_DataAcess.CR_HRPortal;

public partial class AzureServiceMappingTable
{
    public int Id { get; set; }

    public string IndustryType { get; set; } = null!;

    public string CapabilitiesName { get; set; } = null!;

    public string ServiceType { get; set; } = null!;

    public string ServiceName { get; set; } = null!;

    public bool? Include { get; set; }
}
