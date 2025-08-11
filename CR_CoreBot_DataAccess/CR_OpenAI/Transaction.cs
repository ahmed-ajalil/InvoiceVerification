using System;
using System.Collections.Generic;

namespace CR_CoreBot_DataAccess.CR_OpenAI;

public partial class Transaction
{
    public double? TransectionId { get; set; }

    public string? CustId { get; set; }

    public string? TransectionType { get; set; }

    public string? Date { get; set; }

    public double? WithdrawalAmt { get; set; }

    public double? DepositAmt { get; set; }

    public double? ClosingBalance { get; set; }
}
