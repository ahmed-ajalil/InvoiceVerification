using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CR_CoreBot_DTO;

public partial class CustomerModelDTO
{
    public int Id { get; set; }
    [Required(ErrorMessage = "*The CustomerId field is required.")]
    public int? CustomerId { get; set; }
    [Required(ErrorMessage = "*The ModelName field is required.")]
    public string? ModelName { get; set; }
    [Required(ErrorMessage = "*The ModelDisplayName field is required.")]
    public string? ModelDisplayName { get; set; }
}
