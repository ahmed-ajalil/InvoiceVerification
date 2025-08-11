using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CR_CoreBot_DTO
{
    public class BlobConnectedListDTO
    {
        public int Id { get; set; }
        public string? Filename { get; set; }
        public string? ContentType { get; set; }
        public DateTime? DateTime { get; set; }
        public string? DownloadUrl { get; set; }
        public string? IsProcessed { get; set; }

        public string? AbsoluteUri { get; set; }
        public int? FilesAllowed { get; set; }
    }
}
