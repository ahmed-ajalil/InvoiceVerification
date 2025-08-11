using System;
using System.Collections.Generic;

namespace CR_CoreBot_DataAccess.CR_OpenAI;

public partial class PersonalDetail
{
    public string? CustId { get; set; }

    public string? CustomerName { get; set; }

    public double? AccountNo { get; set; }

    public string? Isfccode { get; set; }

    public string? PermanentAddress { get; set; }

    public string? CommunicationAddress { get; set; }

    public string? Email { get; set; }

    public string? Mobile { get; set; }

    public string? Country { get; set; }

    public string? State { get; set; }

    public string? City { get; set; }

    public double? ZipCode { get; set; }

    public double? BankCode { get; set; }

    public double? BranchCode { get; set; }

    public string? BranchName { get; set; }
}
