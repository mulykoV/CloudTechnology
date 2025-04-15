using Microsoft.AspNetCore.Mvc;
using Azure.AI.TextAnalytics;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly TextAnalyticsClient _textAnalyticsClient;

        public HomeController(TextAnalyticsClient textAnalyticsClient)
        {
            _textAnalyticsClient = textAnalyticsClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RecognizePII(string inputText)
        {
            if (string.IsNullOrWhiteSpace(inputText))
            {
                ViewBag.PiiError = "Будь ласка, введіть текст.";
                return View("Index");
            }

            var entities = _textAnalyticsClient.RecognizePiiEntities(inputText).Value;
            ViewBag.RedactedText = entities.RedactedText;
            ViewBag.PiiEntities = entities;

            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RecognizeMedicalInfo(string inputText)
        {
            if (string.IsNullOrWhiteSpace(inputText))
            {
                ViewBag.MedError = "Будь ласка, введіть текст.";
                return View("Index");
            }

            var batchInput = new List<string> { inputText };
            var healthOperation = await _textAnalyticsClient.StartAnalyzeHealthcareEntitiesAsync(batchInput);
            await healthOperation.WaitForCompletionAsync();

            var results = new List<HealthcareEntity>();
            await foreach (var documentsInPage in healthOperation.Value)
            {
                foreach (var entitiesInDoc in documentsInPage)
                {
                    if (!entitiesInDoc.HasError)
                    {
                        results.AddRange(entitiesInDoc.Entities);
                    }
                }
            }

            ViewBag.MedicalEntities = results;
            return View("Index");
        }

        [HttpPost]
        public IActionResult AnalyzeSentiment(string inputText)
        {
            if (string.IsNullOrWhiteSpace(inputText))
            {
                ViewBag.SentimentError = "Будь ласка, введіть текст.";
                return View("Index");
            }

            var sentimentResult = _textAnalyticsClient.AnalyzeSentiment(inputText);
            ViewBag.Sentiment = sentimentResult.Value.Sentiment.ToString();
            return View("Index");
        }
    }
}
