using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Providers.Interface
{
    public interface IAzureStorageService
    {
        Task<string> UploadToAzureAsync(IFormFile file);
    }
}
