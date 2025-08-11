using System;
using System.Collections.Generic;

namespace CR_CoreBot_DataAccess.CR_OpenAI;

public partial class CustomerOffer
{
    public double? OfferId { get; set; }

    public string? CustId { get; set; }

    public string? OffersName { get; set; }

    public double? RateOfIntrest { get; set; }
}
