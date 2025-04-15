using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Test9.Models;

[Route("InteractiveNotes")]
public class InteractiveNotesController : Controller
{
    private readonly AzureVisionConfig _visionConfig;
    private readonly AzureFaceConfig _faceConfig;
    private readonly HttpClient _client;

    public InteractiveNotesController(IOptions<AzureVisionConfig> visionConfig, IOptions<AzureFaceConfig> faceConfig)
    {
        _visionConfig = visionConfig.Value;
        _faceConfig = faceConfig.Value;
        _client = new HttpClient();
    }

    [HttpGet("Analyze")]
    public IActionResult Analyze()
    {
        return View();
    }

    [HttpPost("Analyze")]
    public async Task<IActionResult> Analyze(IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            ViewBag.Error = "Будь ласка, завантажте зображення.";
            return View();
        }

        var stream = image.OpenReadStream();
        var content = new StreamContent(stream);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _visionConfig.Key);
        var ocrUrl = $"{_visionConfig.Endpoint}vision/v3.2/ocr?language=unk&detectOrientation=true";
        var ocrResponse = await _client.PostAsync(ocrUrl, content);
        var ocrJson = await ocrResponse.Content.ReadAsStringAsync();
        dynamic ocrResult = JsonConvert.DeserializeObject(ocrJson);

        
        stream = image.OpenReadStream();
        content = new StreamContent(stream);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        
        var analysisUrl = $"{_visionConfig.Endpoint}vision/v3.2/analyze?visualFeatures=Categories,Description,Tags,Objects&language=en";
        var analysisResponse = await _client.PostAsync(analysisUrl, content);
        var analysisJson = await analysisResponse.Content.ReadAsStringAsync();
        dynamic analysisResult = JsonConvert.DeserializeObject(analysisJson);

      
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _faceConfig.Key);
        stream = image.OpenReadStream();
        content = new StreamContent(stream);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        var faceUrl = $"{_faceConfig.Endpoint.TrimEnd('/')}/face/v1.0/detect";
        var faceResponse = await _client.PostAsync(faceUrl, content);
        var faceJson = await faceResponse.Content.ReadAsStringAsync();
        dynamic faceResult = JsonConvert.DeserializeObject(faceJson);

        ViewBag.Ocr = ocrResult;
        ViewBag.Analysis = analysisResult;
        ViewBag.Faces = faceResult;

        return View();
    }
}
