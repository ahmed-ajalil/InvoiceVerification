using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CR_CoreBot_DTO
{
    public class ModelFineTuneDataDTO
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public string? Prompt { get; set; }
        public string? Completion { get; set; }
        public string? LabelId { get; set; }
        public string? LabelName { get; set; }
        public string? ModelName { get; set; }
        public DateTime? Datetime { get; set; }
        public string? IsValid { get; set; }
        public string? Url { get; set; }
        public int? UserId { get; set; }
        public string? Username { get; set; }
        public string? Tag {  get; set; }
    }
}
