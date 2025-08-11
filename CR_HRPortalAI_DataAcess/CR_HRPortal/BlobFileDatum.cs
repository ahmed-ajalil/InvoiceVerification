using System;
using System.Collections.Generic;

namespace CR_HRPortalAI_DataAcess.CR_HRPortal;

public partial class BlobFileDatum
{
    public int Id { get; set; }

    public string? FileName { get; set; }

    public int? IsProcessed { get; set; }
}
