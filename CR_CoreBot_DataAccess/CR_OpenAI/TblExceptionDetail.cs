using System;
using System.Collections.Generic;

namespace CR_CoreBot_DataAccess.CR_OpenAI;

public partial class TblExceptionDetail
{
    public int Id { get; set; }

    public string? ExceptionMessage { get; set; }

    public string? ControllerName { get; set; }

    public string? ActionName { get; set; }

    public string? ExceptionStakeTrace { get; set; }

    public DateTime? LogTime { get; set; }
}
