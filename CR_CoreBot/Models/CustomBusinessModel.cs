using System.Collections.Generic;

namespace CR_CoreBot.Models
{
    public class CustomBusinessModel
    {
        public string? summary { get; set; }
        public string suggestion { get; set; }
        public List<string> _documentName { get; set; }
        public List<string> documentSourceURL { get; set; }
        
    }
}
