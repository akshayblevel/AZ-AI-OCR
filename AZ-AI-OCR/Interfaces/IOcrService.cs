using Newtonsoft.Json.Linq;

namespace AZ_AI_OCR.Interfaces
{
    public interface IOcrService
    {
        Task<string?> AnalyseAsync(string imageUrl);

        Task<JObject> ExtractText(string operationLocation);
    }
}
