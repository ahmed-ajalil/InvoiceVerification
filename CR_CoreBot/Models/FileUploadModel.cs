using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace CR_CoreBot.Models
{
    public class FileUploadModel
    {
        public string Credentials { get; set; }
        public List<IFormFile> Images { get; set; }
    }
}
