using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using Test9.Models;

[Route("Vision")]
public class VisionController : Controller
{
    private readonly AzureVisionConfig _visionConfig;
    private readonly AzureFaceConfig _faceConfig;
    private readonly HttpClient _client;

    public VisionController(IOptions<AzureVisionConfig> visionConfig, IOptions<AzureFaceConfig> faceConfig)
    {
        _visionConfig = visionConfig.Value;
        _faceConfig = faceConfig.Value;
        _client = new HttpClient();
    }

    [HttpGet("OCR")]
    public IActionResult OCR()
    {
        return View();
    }

    [HttpPost("OCR")]
    public async Task<IActionResult> OCR(IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            ViewBag.Error = "Будь ласка, завантажте зображення.";
            return View();
        }

        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _visionConfig.Key);

        var url = $"{_visionConfig.Endpoint}vision/v3.2/ocr?language=unk&detectOrientation=true";
        var stream = image.OpenReadStream();
        var content = new StreamContent(stream);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        var response = await _client.PostAsync(url, content);
        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Сталася помилка при обробці зображення.";
            return View();
        }

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<dynamic>(json);
        ViewBag.Result = result;
        return View();
    }

    [HttpGet("ImageAnalysis")]
    public IActionResult ImageAnalysis()
    {
        return View();
    }

    [HttpPost("ImageAnalysis")]
    public async Task<IActionResult> ImageAnalysis(IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            ViewBag.Error = "Будь ласка, завантажте зображення.";
            return View();
        }

        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _visionConfig.Key);

        var url = $"{_visionConfig.Endpoint}vision/v3.2/analyze?visualFeatures=Categories,Description,Color&language=en";
        var stream = image.OpenReadStream();
        var content = new StreamContent(stream);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        var response = await _client.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Сталася помилка при обробці зображення.";
            return View();
        }

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<dynamic>(json);
        ViewBag.Result = result;
        return View();
    }

    [HttpGet("FaceDetection")]
    public IActionResult FaceDetection()
    {
        return View();
    }

    [HttpPost("FaceDetection")]
    public async Task<IActionResult> FaceDetection(IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            ViewBag.Error = "Будь ласка, завантажте зображення.";
            return View();
        }

        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _faceConfig.Key);

        var url = $"{_faceConfig.Endpoint.TrimEnd('/')}/face/v1.0/detect";

        var stream = image.OpenReadStream();
        var content = new StreamContent(stream);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        var response = await _client.PostAsync(url, content);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = $"Сталася помилка при обробці обличчя. Код: {response.StatusCode}<br/>Відповідь: {json}";
            return View();
        }

        var data = JsonConvert.DeserializeObject<List<dynamic>>(json);

        var faces = new List<dynamic>();
        foreach (var item in data)
        {
            faces.Add(new
            {
                rect = item.faceRectangle
            });
        }

        ViewBag.Faces = faces;
        return View();
    }



}
