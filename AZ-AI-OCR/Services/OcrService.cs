using AZ_AI_OCR.Interfaces;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace AZ_AI_OCR.Services
{
    public class OcrService : IOcrService
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpoint;
        private readonly string _apiVersion;

        public OcrService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _endpoint = config["AzureVision:Endpoint"] ?? throw new ArgumentNullException("Endpoint");
            _apiVersion = config["AzureVision:ApiVersion"] ?? "";

            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", config["AzureVision:Key"]);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<string?> AnalyseAsync(string imageUrl)
        {
            var uri = $"{_endpoint}vision/{_apiVersion}/read/analyze";

            HttpResponseMessage response;

            string requestBody = "{\"url\":\"" + imageUrl + "\"}";
            using (StringContent content = new StringContent(requestBody))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await _httpClient.PostAsync(uri, content);
            }

            if (response.IsSuccessStatusCode)
            {
                // Extract Operation-Location header (contains operation ID)
                return response.Headers.GetValues("Operation-Location").FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public async Task<JObject> ExtractText(string operationLocation)
        {
            JObject result;
            string status = "";
            
            do
            {
                HttpResponseMessage response = await _httpClient.GetAsync(operationLocation);
                string contentString = await response.Content.ReadAsStringAsync();
                result = JObject.Parse(contentString);
                status = result["status"]?.ToString() ?? "";

                if (status == "succeeded" || status == "failed")
                    break;

                Thread.Sleep(1000); 
            } while (true);

            return result;
        }
    }
}
