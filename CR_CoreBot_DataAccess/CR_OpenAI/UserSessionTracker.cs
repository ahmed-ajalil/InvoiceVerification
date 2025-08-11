using System;
using System.Collections.Generic;

namespace CR_CoreBot_DataAccess.CR_OpenAI;

public partial class UserSessionTracker
{
    public int SessionTrackerId { get; set; }

    public TimeSpan SessionDuration { get; set; }

    public string Username { get; set; } = null!;

    public string LoginWith { get; set; } = null!;

    public DateTime LoginTime { get; set; }

    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public int? TotalPrompt { get; set; }

    public int? TotalToken { get; set; }

    public DateTime CreatedDateTime { get; set; }
}
