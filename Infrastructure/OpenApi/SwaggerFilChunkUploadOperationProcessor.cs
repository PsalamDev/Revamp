using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.OpenApi
{
    public class SwaggerFilChunkUploadOperationProcessor : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            var data = context.OperationDescription.Operation.Parameters;

            //File upload
            data.Add(new NSwag.OpenApiParameter()
            {
                IsRequired = true,
                Name = "file",
                Description = "filechunk",
                Type = JsonObjectType.File,
                Kind = OpenApiParameterKind.FormData
            });

            //custom formdata (not needed for the file upload)
            data.Add(new OpenApiParameter()
            {
                IsRequired = true,
                Name = "file-name",
                Description = "the original file name",
                Type = JsonObjectType.String,
                Kind = OpenApiParameterKind.FormData
            });

            return true;
        }


    }
}