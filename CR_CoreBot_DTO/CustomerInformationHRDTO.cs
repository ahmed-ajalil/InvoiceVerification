using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CR_CoreBot_DTO;

public partial class CustomerInformationHRDTO
{
    public int CustomerId { get; set; }
    [Required(ErrorMessage = "*The UserName field is required.")]
    public string? UserName { get; set; }
    [Required(ErrorMessage = "*The Password field is required.")]
    public string? Password { get; set; }
    [Required(ErrorMessage = "*The Name field is required.")]
    public string? Name { get; set; }
    [Required(ErrorMessage = "*The OrganizationName field is required.")]
    public string? OrganizationName { get; set; }
    [Required(ErrorMessage = "*The OrganizationLogo field is required.")]
    public string? OrganizationLogo { get; set; }
    [Required(ErrorMessage = "*The DatabaseName field is required.")]
    public string? DatabaseName { get; set; }
    [Required(ErrorMessage = "*The Category field is required.")]
    public string? Category { get; set; }

    public DateTime? DateTime { get; set; }
}
