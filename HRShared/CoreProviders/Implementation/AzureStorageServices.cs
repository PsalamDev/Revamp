using Azure.Storage.Blobs;
using HRShared.CoreProviders.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HRShared.CoreProviders.Implementation
{
    public class AzureStorageServices : IAzureStorageServices
    {
        private readonly BlobServiceClient _blobServiceClient;
        public AzureStorageServices(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }
        public async Task<string> UploadToAzureAsync(IFormFile file)
        {
            //_blobServiceClient.
            //var container = _blobServiceClient.CreateBlobContainerAsync();
            var blobContainer = _blobServiceClient.GetBlobContainerClient("multi");
            var blobClient = blobContainer.GetBlobClient(DateTime.Now.ToString("yyyyyMMddhhmmss") + "_" + file.FileName);
            var res = await blobClient.UploadAsync(file.OpenReadStream(), overwrite: true);



            return blobClient.Uri.AbsoluteUri;
        }
    }
}
