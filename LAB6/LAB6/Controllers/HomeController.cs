using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

public class HomeController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly AzureSettings _azureSettings;

    public HomeController(HttpClient httpClient, IOptions<AzureSettings> azureSettings)
    {
        _httpClient = httpClient;
        _azureSettings = azureSettings.Value;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> NER(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            ViewBag.Message = "Будь ласка, введіть текст.";
            return View("Index");
        }

        var requestJson = JsonSerializer.Serialize(new
        {
            documents = new[] { new { id = "1", language = "en", text } }
        });

        using var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_azureSettings.Endpoint}/text/analytics/v3.1/entities/recognition/general")
        {
            Content = requestContent
        };

        request.Headers.Add("Ocp-Apim-Subscription-Key", _azureSettings.ApiKey);

        try
        {
            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Message = $"Помилка при отриманні даних від Azure: {response.StatusCode}";
                return View("Index");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var nerResult = JsonSerializer.Deserialize<AzureNERResponse>(responseJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var entities = nerResult?.Documents?.FirstOrDefault()?.Entities ?? new List<Entity>();
            ViewBag.Entities = entities;
            TempData["Entities"] = JsonSerializer.Serialize(entities);
        }
        catch (HttpRequestException ex)
        {
            ViewBag.Message = $"Помилка HTTP-запиту: {ex.Message}";
        }
        catch (JsonException ex)
        {
            ViewBag.Message = $"Помилка обробки JSON: {ex.Message}";
        }

        return View("Index");
    }

    [HttpPost]
    public IActionResult GenerateTest()
    {
        if (TempData["Entities"] == null)
        {
            ViewBag.Message = "Помилка: немає розпізнаних сутностей для створення тесту.";
            return View("Index");
        }

        var entities = JsonSerializer.Deserialize<List<Entity>>(TempData["Entities"] as string);
        TempData.Keep("Entities"); 

        var questions = new List<TestQuestion>();
        var random = new Random();
        int idCounter = 1;

        foreach (var entity in entities)
        {
            var wrongOptions = entities
                .Where(e => e.Category != entity.Category)
                .Select(e => e.Category)
                .Distinct()
                .Take(2)
                .ToList();

            while (wrongOptions.Count < 2)
            {
                wrongOptions.Add("Категорія " + random.Next(1, 10));
            }

            var options = wrongOptions.Concat(new List<string> { entity.Category })
                                      .OrderBy(_ => random.Next())
                                      .ToList();

            questions.Add(new TestQuestion
            {
                Id = idCounter++,
                Text = $"До якої категорії належить: \"{entity.Text}\"?",
                CorrectAnswer = entity.Category,
                Options = options
            });
        }

        TempData["TestQuestions"] = JsonSerializer.Serialize(questions);
        TempData.Keep("TestQuestions"); 

        ViewBag.TestQuestions = questions;
        return View("Index");
    }


    [HttpPost]
    public IActionResult CheckTest(IFormCollection form)
    {
        if (TempData["TestQuestions"] == null)
        {
            ViewBag.Message = "Помилка: тестові питання відсутні.";
            return View("Index");
        }

        var correctAnswers = JsonSerializer.Deserialize<List<TestQuestion>>(TempData["TestQuestions"] as string);
        TempData.Keep("TestQuestions");

        if (correctAnswers == null || correctAnswers.Count == 0)
        {
            ViewBag.Message = "Помилка: не вдалося завантажити тестові питання.";
            return View("Index");
        }

        int score = 0;
        foreach (var question in correctAnswers)
        {
            if (form.TryGetValue(question.Id.ToString(), out var userAnswer))
            {
                question.UserAnswer = userAnswer;
                if (userAnswer == question.CorrectAnswer)
                {
                    score++;
                }
            }
            else
            {
                question.UserAnswer = null;
            }
        }

        ViewBag.Score = score;
        ViewBag.TotalQuestions = correctAnswers.Count;
        ViewBag.TestQuestions = correctAnswers; // Передаємо оновлені питання у View
        return View("Index");
    }

}
public class AzureNERResponse
{
    public List<Document> Documents { get; set; }
}

public class Document
{
    public List<Entity> Entities { get; set; }
}

public class Entity
{
    public string Category { get; set; }
    public string Text { get; set; }
}

public class TestQuestion
{
    public int Id { get; set; }
    public string Text { get; set; }
    public string CorrectAnswer { get; set; }
    public List<string> Options { get; set; }
    public string UserAnswer { get; set; }
}

