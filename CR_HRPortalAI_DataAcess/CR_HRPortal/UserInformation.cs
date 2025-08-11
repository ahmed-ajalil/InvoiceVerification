using System;
using System.Collections.Generic;

namespace CR_HRPortalAI_DataAcess.CR_HRPortal;

public partial class UserInformation
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public int? CustomerId { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? LoginType { get; set; }
    public int? Streaming { get; set; }
}
