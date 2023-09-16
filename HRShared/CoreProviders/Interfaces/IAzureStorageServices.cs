using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRShared.CoreProviders.Interfaces
{
    public interface IAzureStorageServices
    {
        Task<string> UploadToAzureAsync(IFormFile file);
    }
}
