using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CR_CoreBot_DataAccess.CR_OpenAI
{
    public partial class Streaming
    {
        public int? id { get; set; }
        public string? Track_Number { get; set; }
        public string? Prompt { get; set; }
        public string? Logs { get; set; }
        public DateTime? Created_at { get; set; }
        public int? Status { get; set; }
    }
}
