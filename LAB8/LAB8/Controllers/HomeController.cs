using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Azure.AI.Translation.Text;
using Azure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TranslationApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly TextTranslationClient _translatorClient;

        public HomeController(TextTranslationClient translatorClient)
        {
            _translatorClient = translatorClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Translate(string inputText, string targetLanguage)
        {
            if (string.IsNullOrEmpty(inputText) || string.IsNullOrEmpty(targetLanguage))
            {
                ViewBag.Error = "Будь ласка, введіть текст і виберіть мову перекладу.";
                return View("Index");
            }

            try
            {
                Console.WriteLine($"Input Text: {inputText}");
                Console.WriteLine($"Target Language: {targetLanguage}");

                Response<IReadOnlyList<TranslatedTextItem>> response =
                    await _translatorClient.TranslateAsync(targetLanguage, inputText).ConfigureAwait(false);

                var translation = response.Value.FirstOrDefault();

                ViewBag.InputText = inputText;
                ViewBag.TargetLanguage = targetLanguage;
                ViewBag.TranslatedText = translation?.Translations?.FirstOrDefault()?.Text ?? "Помилка перекладу.";
            }
            catch (RequestFailedException ex)
            {
                ViewBag.Error = $"Помилка: {ex.Message}";
            }

            return View("Index");
        }


        [HttpPost]
        public async Task<IActionResult> TranslateFile(IFormFile file, string targetLanguage)
        {
            if (file == null || file.Length == 0 || string.IsNullOrEmpty(targetLanguage))
            {
                ViewBag.Error = "Будь ласка, завантажте файл та виберіть мову перекладу.";
                return View("Index");
            }

            try
            {
                using var reader = new StreamReader(file.OpenReadStream());
                string fileContent = await reader.ReadToEndAsync();

                Response<IReadOnlyList<TranslatedTextItem>> response =
                    await _translatorClient.TranslateAsync(targetLanguage, fileContent).ConfigureAwait(false);

                var translation = response.Value.FirstOrDefault();

                ViewBag.FileContent = fileContent;
                ViewBag.TranslatedFileText = translation?.Translations?.FirstOrDefault()?.Text ?? "Помилка перекладу.";
                ViewBag.TargetLanguage = targetLanguage;
            }
            catch (RequestFailedException ex)
            {
                ViewBag.Error = $"Помилка: {ex.Message}";
            }

            return View("Index");
        }
    }
}
