using System;
using System.Collections.Generic;

namespace CR_CoreBot_DataAccess.CR_OpenAI;

public partial class TblModelConfiguration
{
    public int Id { get; set; }

    public string? ConfigurationText { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? Industry { get; set; }
}
