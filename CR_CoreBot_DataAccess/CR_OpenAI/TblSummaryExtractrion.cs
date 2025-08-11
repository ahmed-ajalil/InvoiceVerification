using System;
using System.Collections.Generic;

namespace CR_CoreBot_DataAccess.CR_OpenAI;

public partial class TblSummaryExtractrion
{
    public int Id { get; set; }

    public string? SummaryDescription { get; set; }

    public string? FileName { get; set; }

    public int? FileId { get; set; }

    public DateTime? CreatedDate { get; set; }
}
