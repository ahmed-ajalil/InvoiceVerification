using System;
using System.Collections.Generic;

namespace CR_CoreBot_DataAccess.CR_OpenAI;

public partial class CustomerConfiguration
{
    public int ConfigurationId { get; set; }

    public int CustomerId { get; set; }

    public string Username { get; set; } = null!;

    public int UsersAllowed { get; set; }

    public int? FilesAllowed { get; set; }

    public DateTime CreateDateTime { get; set; }

    public string FileFormat { get; set; } = null!;

    public string? LoginType { get; set; }
}
