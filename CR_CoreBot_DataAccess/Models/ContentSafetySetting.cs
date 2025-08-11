using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CR_CoreBot_DataAccess.Models
{
    public class ContentSafetySettings
    {
        [Key]
        public int Id { get; set; }
        public int selfHarmRejectionThreshold { get; set; }
        public int violenceRejectionThreshold { get; set; }
        public int hateRejectionThreshold { get; set; }
        public int sexualRejectionThreshold { get; set; }
        public string? UserId { get; set; }
    }
}
