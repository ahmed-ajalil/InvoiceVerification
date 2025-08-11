using System;
using System.Collections.Generic;

namespace CR_CoreBot_DataAccess.CR_OpenAI;

public partial class ChatBotKeyConfiguration
{
    public int Id { get; set; }

    public string? EndPoint { get; set; }

    public string? ApiKey { get; set; }

    public string? ServiceName { get; set; }

    public DateTime? DateTime { get; set; }
}
