using Azure.Storage.Blobs;
using Infrastructure.Providers.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Providers.Implementation
{
    public class AzureStorageService : IAzureStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        public AzureStorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }
        public async Task<string> UploadToAzureAsync(IFormFile file)
        {
            //_blobServiceClient.
            //var container = _blobServiceClient.CreateBlobContainerAsync();
            var blobContainer = _blobServiceClient.GetBlobContainerClient("multi");
            var blobClient = blobContainer.GetBlobClient(DateTime.Now.ToString("yyyyyMMddhhmmss") + "_" + file.FileName);
            var res = await blobClient.UploadAsync(file.OpenReadStream());



            return blobClient.Uri.AbsoluteUri;
        }
    }
}
