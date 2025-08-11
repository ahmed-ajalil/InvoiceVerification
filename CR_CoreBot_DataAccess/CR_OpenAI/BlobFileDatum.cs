using System;
using System.Collections.Generic;

namespace CR_CoreBot_DataAccess.CR_OpenAI;

public partial class BlobFileDatum
{
    public int Id { get; set; }

    public string? FileName { get; set; }

    public int? IsProcessed { get; set; }

    public int? CustomerId { get; set; }
}
