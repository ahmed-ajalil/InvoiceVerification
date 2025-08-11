
namespace CR_CoreBot_DTO
{
    public class AzureDTO
    {
        public List<SourceDocument> source_document { get; set; }
        //public List<string> source_document { get; set; }
        public string qa_result { get; set; }
        public string qa_result1 { get; set; }
        public string total_tokens { get; set; }
        public string prompt_tokens { get; set; }
        public string Completion_tokens { get; set; }
        public string StructuredData_Context { get; set; }
        public decimal total_cost { get; set; }
        public List<string> suggestions { get; set; }
       // public List<string> Items { get; set; }

    }
    public class SourceDocument
    {
        public string tool { get; set; }
        public string source { get; set; }
        public string response { get; set; }
        public string query { get; set; }
        public string content { get; set; }
        public decimal search_score { get; set; }
    }
}
