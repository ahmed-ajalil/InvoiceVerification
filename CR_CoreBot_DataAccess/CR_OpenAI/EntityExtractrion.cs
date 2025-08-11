using System;
using System.Collections.Generic;

namespace CR_CoreBot_DataAccess.CR_OpenAI;

public partial class EntityExtractrion
{
    public int Id { get; set; }

    public string? Entity { get; set; }

    public string? EntityDescription { get; set; }

    public string? FileName { get; set; }

    public string? ImportantNotes { get; set; }

    public string? Icdcode { get; set; }

    public int? FileId { get; set; }

    public DateTime? CreatedDate { get; set; }
}
