using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CR_CoreBot_DTO;

public partial class ChatBotKeyConfigurationDTO
{
    public int? Id { get; set; }
    [Required(ErrorMessage = "*The EndPoint field is required.")]
    public string? EndPoint { get; set; }
    [Required(ErrorMessage = "*The ApiKey field is required.")]
    public string? ApiKey { get; set; }
    [Required(ErrorMessage = "*The ServiceName field is required.")]
    public string? ServiceName { get; set; }
    public DateTime? DateTime { get; set; }
}
