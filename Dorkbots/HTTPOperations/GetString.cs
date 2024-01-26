using System;
using System.Net.Http;

namespace Dorkbots.HTTPOperations
{
    public class GetString
    {
        public GetString(string pathToJSON, Action<string> callback)
        {
            PerformGet(pathToJSON, callback);
        }

        private async void PerformGet(string pathToJson, Action<string> callback)
        {
            HTTPGetOperation getOperation = new HTTPGetOperation();
            HttpResponseMessage response = await getOperation.Get(pathToJson);

            string responseString = await response.Content.ReadAsStringAsync();

            callback?.Invoke(responseString);
        }
    }
}