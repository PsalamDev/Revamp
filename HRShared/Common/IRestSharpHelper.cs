using Newtonsoft.Json;
using RestSharp;

namespace HRShared.Common
{
    public interface IRestSharpHelper<R, M> where R : class where M : class
    {
        Task<R> Get(string endpoint, Dictionary<string, string>? headers = null);
        Task<R> Post(string endpoint, M? model = null, Dictionary<string, string>? headers = null);
    }

    public class RestSharpHelper<R, M> : IRestSharpHelper<R, M> where R : class where M : class
    {
        public RestSharpHelper()
        {
        }

        public async Task<R> Get(string endpoint, Dictionary<string, string>? headers = null)
        {
            try
            {

                var client = new RestClient();
                var request = new RestRequest(endpoint, Method.Get);


                if (headers != null)
                {
                    request.AddHeaders(headers);
                }
                var requestResponse = await client.ExecuteGetAsync(request);

                if (requestResponse.IsSuccessful)
                {
                    var response = JsonConvert.DeserializeObject<R>(requestResponse.Content);
                    return response;
                }

                return null;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<R> Post(string endpoint, M? model = null, Dictionary<string, string>? headers = null)
        {
            try
            {

                var client = new RestClient();
                var request = new RestRequest(endpoint, Method.Post);


                if (headers != null)
                {
                    request.AddHeaders(headers);
                }

                if (model != null)
                {
                    request.AddJsonBody(model);
                }

                var requestResponse = await client.ExecutePostAsync(request);

                if (requestResponse.IsSuccessful)
                {
                    var response = JsonConvert.DeserializeObject<R>(requestResponse.Content);
                    return response;
                }

                return null;

            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}