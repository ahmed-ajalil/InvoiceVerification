using System;
using System.Collections.Generic;

namespace CR_HRPortalAI_DataAcess.CR_HRPortal;

public partial class ModelFineTuneDatum
{
    public int Id { get; set; }

    public int? CustomerId { get; set; }

    public string? Prompt { get; set; }

    public string? Completion { get; set; }

    public string? LabelId { get; set; }

    public string? LabelName { get; set; }

    public string? ModelName { get; set; }

    public DateTime? Datetime { get; set; }

    public string? IsValid { get; set; }

    public string? Url { get; set; }

    public int? UserId { get; set; }

    public string? Username { get; set; }

    public string? Tag { get; set; }
}
