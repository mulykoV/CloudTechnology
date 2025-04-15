using Microsoft.AspNetCore.Mvc;
using Azure.AI.Translation.Text;
using Azure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TranslationApp.Controllers
{
    public class LearningController : Controller
    {
        private readonly TextTranslationClient _translatorClient;

        public LearningController(TextTranslationClient translatorClient)
        {
            _translatorClient = translatorClient;
        }

        public IActionResult Index()
        {
            var books = new List<string> { "Цвіт Яблуні", "Intermezzo", "Хемінгвей", "Казка Стівена Кінга" };
            return View(books);
        }

        public IActionResult Book(string title)
        {
            var bookText = GetBookText(title);
            ViewBag.BookTitle = title;
            return View("Book", bookText);
        }

        [HttpPost]
        public async Task<IActionResult> TranslateWord(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return Json(new { translation = "Помилка" });
            }

            var response = await _translatorClient.TranslateAsync("uk", word).ConfigureAwait(false);
            var translation = response.Value.FirstOrDefault()?.Translations?.FirstOrDefault()?.Text ?? "Помилка перекладу";

            return Json(new { translation });
        }

        private string GetBookText(string title)
        {
            var books = new Dictionary<string, string>
            {
                { "Цвіт Яблуні", "Etude\r\n\r\nI closed the door to my office tightly. I can't... I decidedly cannot hear that suppressed, whistling breath that seems to fill the whole house. There, in the woman's bedroom, my child dies. I walk around my office, I have been walking for the third sleepless night, audible like a tuned harp, sounding with strings from every movement of the air. My lamp under a wide cardboard lampshade divides the house into two floors — dark, gloomy, heavy at the top, below it — flooded with light, with clear glitter and with a grid of shadows. A bed sent on a couch and untouched is especially painful to the eye. Behind the black windows lies a world flooded by night, and my house seems to me to be the cabin of a ship sailing somewhere in an unknown black sea with me, with my longing and with my horror. It is strange to me that I notice everything, although grief took me away completely, captivated me. I even, passing by the table, corrected the photo. O! Now symmetrical.. A whistle does not subside. I hear him through the closed door. I won't go to the bedroom. What? I see everything anyway, I see my little girl, her bare hands on the row; I see how her breasts walk under the row, how she opens her baked lips and catches air. That little one, usually so wild, now hugs the doctor's neck with his plump hands and opens his mouth of his own. So submissive now, kitten... This breaks my heart. When would the end be sooner..\r\n\r\nI'm listening. The slightest rustle or knock — and my heart falls and freezes. It seems to me that something unusual is about to happen: a creature with large black wings will enter through the window, a shadow will move through the house, or someone will suddenly scream — and life will be cut short. I'm listening. No, the house does not sleep. Something big, unknown lives in it. I hear her breathing, sighing, restlessly pounding his heart and beating alive. I know it's anxiety. She even holds the air of home in her arms, and she wants to break out from under her oppression, leave the house and throw it off you.." },
                { "Intermezzo", "All that was left was to pack... It was one of those countless \"musts\" that tired me so much and kept me awake. It doesn't matter whether the \"must\" is small or big - what matters is that each time it demands attention, that it is not I who eats it, but it already controls me. In fact, you become a slave to this many-headed beast. At least for a while to get away from it, to forget, to rest. I am tired.\r\n\r\nBecause life is relentlessly and inexorably coming at me, like a wave on the shore. Not only my own, but also someone else's. And in the end - do I know where my own life ends and someone else's begins? I hear someone else's existence entering mine, like air through windows and doors, like water from tributaries into a river. I cannot miss a person. I cannot be, alone. I confess - I envy the planets: they have their own orbits, and nothing stands in their way. Whereas in my country I always meet people everywhere." },
                { "Старий і море", "He was an old man who fished alone in a skiff in the Gulf Stream and he had gone eighty-four days now without taking a fish. In the first forty days a boy had been with him. But after forty days without a fish the boy's parents had told him that the old man was now definitely and finally salao, which is the worst form of unlucky, and the boy had gone at their orders in another boat which caught three good fish the first week. It made the boy sad to see the old man come in each day with his skiff empty and he always went down to help him carry either the coiled lines or the gaff and harpoon and the sail that was furled around the mast. The sail was patched with flour sacks and, furled, it looked like the flag of permanent defeat." },
                { "Казка", "Once upon a time there was a boy called Charlie. His mother died in a terrible accident when he was young, and his father turned to drink, but Charlie grew up to be a good, strong, clever young man. The sort who helps strangers in need – such as the misanthropic Mr Bowditch, who has an equally elderly dog and a crumbling property. Which, as this is a Stephen King novel, sits on a tunnel leading to a mysterious world in need of saving from a horrifying evil." }
            };

            return books.TryGetValue(title, out var text) ? text : "Текст не знайдено.";
        }
    }
}
