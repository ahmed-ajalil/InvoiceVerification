using System;
using System.Collections.Generic;

namespace CR_CoreBot_DataAccess.CR_OpenAI;

public partial class BlobConnection
{
    public int Id { get; set; }

    public int? CustomerId { get; set; }

    public string? UserName { get; set; }

    public string? AccountName { get; set; }

    public string? AccountKey { get; set; }

    public string? BlobContainerName { get; set; }

    public string? BlobName { get; set; }

    public string? EndpointSuffix { get; set; }

    public string? ConnectionString { get; set; }

    public string? StorageType { get; set; }

    public DateTime? DateTime { get; set; }
}
