using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CR_CoreBot_DTO;

public partial class BlobConnectionModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "*The CustomerId field is required.")]
    public int? CustomerId { get; set; }
    [Required(ErrorMessage = "*The AccountName field is required.")]
    public string? AccountName { get; set; }
    [Required(ErrorMessage = "*The AccountKey field is required.")]
    public string? AccountKey { get; set; }
    [Required(ErrorMessage = "*The BlobContainerName field is required.")]
    public string? BlobContainerName { get; set; }
    [Required(ErrorMessage = "*The BlobName field is required.")]
    public string? BlobName { get; set; }
    [Required(ErrorMessage = "*The EndpointSuffix field is required.")]
    public string? EndpointSuffix { get; set; }
    [Required(ErrorMessage = "*The ConnectionString field is required.")]
    public string? StorageType { get; set; }
    public string? ConnectionString { get; set; }

    public DateTime? DateTime { get; set; }
}
