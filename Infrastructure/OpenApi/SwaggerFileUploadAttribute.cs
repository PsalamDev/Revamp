using Infrastructure.OpenApi;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.OpenApi
{
    public class SwaggerFileUploadAttribute : SwaggerOperationProcessorAttribute
    {
        public SwaggerFileUploadAttribute() : base(typeof(SwaggerFilChunkUploadOperationProcessor))
        {
        }
    }
}
