using AZ_AI_OCR.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AZ_AI_OCR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OcrController(IOcrService ocrService) : ControllerBase
    {
        [HttpPost("ExtractText")]
        public async Task<IActionResult> ExtractText([FromBody] string imageUrl)
        {
            string operationLocation = await ocrService.AnalyseAsync(imageUrl) ?? "";

            if (!string.IsNullOrEmpty(operationLocation))
            {
                JObject result = await ocrService.ExtractText(operationLocation);
                return Content(result.ToString(Newtonsoft.Json.Formatting.None), "application/json");
            }
            return Content("", "application/json");
        }
    }
}
