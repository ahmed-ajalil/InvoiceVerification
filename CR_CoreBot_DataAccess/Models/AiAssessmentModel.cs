using CR_CoreBot_DataAccess.CR_OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CR_CoreBot_DataAccess.Models
{
    public class AiAssessmentModel
    {
        public List<assessmentScore>? aiAssessmentModel { get; set; }
        public List<PerformanceMatrixChecker> performanceMatrixCheckers { get; set; }
        public List<string> drpdown { get; set; }
    }
    public class assessmentScore
    {
        public string type { get; set; }
        public double? score { get; set; }
    }
}
