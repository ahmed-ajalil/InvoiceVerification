using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CR_CoreBot_DTO
{
    public class FlightValidationDataDTO
    {
        public DateTime? FlightDate { get; set; }
        public string? FlightNumber { get; set; }
        public string? Dep { get; set; }
        public string? Arr { get; set; }
        public string? BCMeals { get; set; }
        public string? ValidationStatus { get; set; }
        public string? FileName { get; set; }
        public int? FileId { get; set; }
        public float? InternalTotalBCMealsCost { get; set; }

    }
}
