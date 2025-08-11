using Azure;
using Azure.Identity;
using Azure.Search.Documents.Indexes;
using Azure.Storage.Blobs;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using System;
using System.IO;

namespace CR_CoreBot.common
{
    internal class AIServicesDependent
    {
        public SearchIndexerClient searchIndexerClient(string _aiSearchName,string _managedIdentityClientId)
        {
            try
            {
                var searchAdminauth = new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    ManagedIdentityClientId = _managedIdentityClientId
                });

                SearchIndexerClient _indexerClient = new SearchIndexerClient(new Uri($"https://{_aiSearchName}.search.windows.net"), searchAdminauth);
                return _indexerClient;  
            }
            catch(Exception ex)
            {
                 return null;
            }
            
        }
        public BlobServiceClient blobServiceClient(string StorageName , string _managedIdentityClientId)
        {
            try
            {
                DefaultAzureCredential searchAdminauth = new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    ManagedIdentityClientId = _managedIdentityClientId
                });

                BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri($"https://{StorageName}.blob.core.windows.net/"), searchAdminauth);
                return blobServiceClient;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


    }
}
