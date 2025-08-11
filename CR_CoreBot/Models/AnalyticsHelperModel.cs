using System.Collections.Generic;

namespace CR_CoreBot.Models
{
    public class AnalyticsHelperModel
    {
        public string? Language { get; set; }
        public string? Sentiment { get; set; }
        public string? Phrases { get; set; }
        public string? summary { get; set; }
        public string? Transcript { get; set; }
        public List<string> EntityRole { get; set; }
        public List<string> PhrasesRole { get; set; }
        public List<string> Entity { get; set; }
        public List<string> _documentName { get; set; }
        public List<string> documentSourceURL { get; set; }
        public string suggestion { get; set; }
    }
}
