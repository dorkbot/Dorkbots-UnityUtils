using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dorkbots.HTTPOperations
{
    public class HTTPGetOperation
    {
        public async Task<HttpResponseMessage> Get(string pathToJson)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => { return true; };//force cert
            HttpClient client = new HttpClient(handler);

            try
            {
                HttpResponseMessage response = await client.GetAsync(pathToJson);
                response.EnsureSuccessStatusCode();

                return response;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Error from URI " + pathToJson + " " + e);
                throw;
            }
        }
    }
}