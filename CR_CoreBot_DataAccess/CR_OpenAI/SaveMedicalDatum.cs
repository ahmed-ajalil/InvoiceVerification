using System;
using System.Collections.Generic;

namespace CR_CoreBot_DataAccess.CR_OpenAI;

public partial class SaveMedicalDatum
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Date { get; set; }

    public string? History { get; set; }

    public string? TestsDone { get; set; }

    public string? Medications { get; set; }

    public string? BodyExaminations { get; set; }

    public string? EntityExtracted { get; set; }

    public string? PromptInput { get; set; }

    public string? Summarise { get; set; }

    public string? IsMarked { get; set; }
}
