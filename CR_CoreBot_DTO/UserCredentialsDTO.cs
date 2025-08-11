using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CR_CoreBot_DTO
{
    public class UserCredentialsDTO
    {
        public string? sourceAccountName { get; set; }
        public string? sourceAccountKey { get; set; }
        public string? sourceBlobContainerName { get; set; }
        public string? sourceConnectionString { get; set; }
        public string? sourcePersonalAccessToken { get; set; }
        public string? sourceRepositoryURL { get; set; }
        public string? sourceClientID { get; set; }
        public string? sourceRedirectURI { get; set; }
        public string? sourcebucketName { get; set; }
        public string? sourcekeyName { get; set; }
        public string? blobmsg { get; set; }
        public string? Filestatus { get; set; }
    }
}
