using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace CR_CoreBot_DTO
{
    public class OpenAIIndexModel
    {
        [SimpleField(IsKey = true, IsFilterable = true)]
        public string chunk_id { get; set; }

        [SearchableField(IsSortable = true, IsFilterable = true)]
        public string parent_id { get; set; }

        [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnLucene, IsFilterable = true)]
        public string chunk { get; set; }

        [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnLucene)]
        public string title { get; set; }
    }
}
