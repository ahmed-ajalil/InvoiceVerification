using System;
using System.Collections.Generic;

namespace CR_CoreBot_DataAccess.CR_OpenAI;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int OveralExperience { get; set; }

    public string? Satisfactionwithbot { get; set; }

    public string? SuggestionFeedback { get; set; }

    public string EmailFeedback { get; set; } = null!;

    public DateTime FeedbackDate { get; set; }

    public int CustomerId { get; set; }

    public int? UserId { get; set; }
}
