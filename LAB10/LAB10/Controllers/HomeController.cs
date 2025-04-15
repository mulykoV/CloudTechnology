using Microsoft.Identity.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using LAB10.Models;
using LAB9.Models;
using Microsoft.Identity.Client;

namespace LAB10.Controllers
{
    public class HomeController : Controller
    {
        private readonly string TenantId;     // Azure subscription TenantId
        private readonly string ClientId;     // Microsoft Entra ApplicationId
        private readonly string ClientSecret; // Microsoft Entra Application Service Principal password
        private readonly string Subdomain;    // Immersive Reader resource subdomain (resource 'Name' if the resource was created in the Azure portal, or 'CustomSubDomain' option if the resource was created with Azure CLI PowerShell. Check the Azure portal for the subdomain on the Endpoint in the resource Overview page, for example, 'https://[SUBDOMAIN].cognitiveservices.azure.com/')

        private IConfidentialClientApplication _confidentialClientApplication;
        private IConfidentialClientApplication ConfidentialClientApplication
        {
            get
            {
                if (_confidentialClientApplication == null)
                {
                    _confidentialClientApplication = ConfidentialClientApplicationBuilder.Create(ClientId)
                    .WithClientSecret(ClientSecret)
                    .WithAuthority($"https://login.windows.net/{TenantId}")
                    .Build();
                }

                return _confidentialClientApplication;
            }
        }

        public HomeController(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            TenantId = configuration["ImmersiveReader:TenantId"];
            ClientId = configuration["ImmersiveReader:ClientId"];
            ClientSecret = configuration["ImmersiveReader:ClientSecret"];
            Subdomain = configuration["ImmersiveReader:Subdomain"];

            if (string.IsNullOrWhiteSpace(TenantId))
            {
                throw new ArgumentNullException("TenantId is null! Did you add that info to secrets.json?");
            }

            if (string.IsNullOrWhiteSpace(ClientId))
            {
                throw new ArgumentNullException("ClientId is null! Did you add that info to secrets.json?");
            }

            if (string.IsNullOrWhiteSpace(ClientSecret))
            {
                throw new ArgumentNullException("ClientSecret is null! Did you add that info to secrets.json?");
            }

            if (string.IsNullOrWhiteSpace(Subdomain))
            {
                throw new ArgumentNullException("Subdomain is null! Did you add that info to secrets.json?");
            }
        }

        /// <summary>
        /// Get a Microsoft Entra ID authentication token
        /// </summary>
        public async Task<string> GetTokenAsync()
        {
            const string resource = "https://cognitiveservices.azure.com/";

            var authResult = await ConfidentialClientApplication.AcquireTokenForClient(
                new[] { $"{resource}/.default" })
                .ExecuteAsync()
                .ConfigureAwait(false);

            return authResult.AccessToken;
        }

        [HttpGet]
        public async Task<JsonResult> GetTokenAndSubdomain()
        {
            try
            {
                string tokenResult = await GetTokenAsync();

                return new JsonResult(new { token = tokenResult, subdomain = Subdomain });
            }
            catch (Exception e)
            {
                string message = "Unable to acquire Microsoft Entra token. Check the console for more information.";
                Debug.WriteLine(message, e);
                return new JsonResult(new { error = message });
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Book(string id)
        {
            string title = "";
            string content = "";

            switch (id?.ToLower())
            {
                case "appletree":
                    title = "Blossom of the Apple Tree";
                    content = @"Я щільно причинив двері од свого кабінету. Я не можу… я рішуче не можу чути того здушеного, з присвистом віддиху, що, здається, сповняв собою весь дім. Там, у жінчиній спальні, вмирає моя дитина. Я ходжу по свому кабінету, ходжу вже третю безсонну ніч, чуткий, як настроєна арфа, що гучить струнами од кожного руху повітря. Моя лампа під широким картоновим абажуром ділить хату на два поверхи — вгорі темний, похмурий, важкий, під ним — залитий світлом, із ясними блисками і з сіткою тіней. Послана на кушетці й неторкана постіль особливо ріже око. За чорними вікнами лежить світ, затоплений ніччю, a моя хата здається мені каютою корабля, що пливе десь у невідомому чорному морі разом зо мною, з моєю тугою і з моїм жахом. Мені дивно, що я усе помічаю, хоч горе забрало мене цілком, полонило. Я навіть, проходячи повз стіл, поправив фотографію. О! Тепер симетрично!.. A свист не вгаває. Я його чую й крізь зачинені двері. Я не піду до спальні. Чого? Я й так бачу все, бачу свою дівчинку, її голі ручки на рядні; бачу, як ходять під рядном її груди, як вона розтулює спечені губи й ловить повітря. Оте мале, звичайно таке дике, тепер обіймає пухкими рученятами шию лікаря й само одкриває рота. Таке покірливе тепер, котенятко… Се мені крає серце. Коли б швидше кінець!..";
                    break;
                case "intermezzo":
                    title = "Intermezzo";
                    content = @"Лишилось тільки ще спакуватись... Се було одно з тих незчисленних ""треба"", які мене так утомили і не давали спати. Дарма, чи те ""треба"" мале, чи велике, — вагу те має, що кожен раз воно вимагає уваги, що не я їм, а воно мною уже керує. Фактично стаєш невільником сього многоголового звіра. Хоч на час увільнитись від нього, забути, спочити. Я утомився.

Бо життя безупинно і невблаганно іде на мене, як хвиля на берег. Не тільки власне, а і чуже. А врешті — хіба я знаю, де кінчається власне життя, а чуже починається? Я чую, як чуже існування входить в моє, мов повітря крізь вікна і двері, як води притоків у річку. Я не можу розминутись з людиною. Я не можу бути, самотнім. Признаюсь — заздрю планетам: вони мають свої орбіти, і ніщо не стає їм на їхній дорозі. Тоді як на своїй я скрізь і завжди стрічаю людину.

Так, ти стаєш мені на дорозі і уважаєш, що маєш на мене право. Ти скрізь. Се ти одягла землю в камінь й залізо, се ти через вікна будинків — тисячі чорних ротів — вічно дихаєш смородом. Ти бичуєш святу тишу землі скреготом фабрик, громом коліс, брудниш повітря пилом та димом, ревеш від болю, з радості, злості. Як звірина. Скрізь я стрічаю твій погляд; твої очі, цікаві, жадні, влазять у мене, і сама ти, в твоїй розмаїтості кольорів й форм, застрягаєш в моїй зіниці. Я не можу розминутись з тобою... я не можу бути самотнім... Ти на тільки йдеш поруч зо мною, ти влазиш всередину в мене. Ти кидаєш у моє серце, як до власного сховку, свої страждання і свої болі, розбиті надії і свою розпач. Свою жорстокість і звірячі інстинкти. Весь жах, весь бруд свого існування. Яке тобі діло, що ти мене мучиш? Ти хочеш буть моїм паном, хочеш взяти мене... мої руки, мій розум, мою волю і моє серце... Ти хочеш виссать мене, всю мою кров, як той вампір. І ти се робиш. Я живу не так, як хочу, а як ти мені кажеш в твоїх незліченних ""треба"", у безконечних ""мусиш"".
";
                    break;
                case "thetale":
                    title = "The Tale by Stephen King";
                    content = @"I was seventeen and knew nothing. Life was a movie I’d just walked into, late. The plot was unclear, the characters were confusing, and I was sure I’d missed something important.
My father had died two years before, and I still hadn’t figured out how to fill the space he left behind. Grief is like a room you keep going back to, even when you know there’s nothing new inside.""
Then one day, I found the key. Not a metaphorical one, but a real, heavy, old-fashioned key in the pocket of a man who shouldn’t have had it. That was the day everything changed — the day I discovered the door, and what was waiting behind it.";
                    break;
                default:
                    title = "Book Not Found";
                    content = "Sorry, the book you are looking for does not exist.";
                    break;
            }

            ViewBag.Title = title;
            ViewBag.Content = content;

            return View("Book");
        }
    }
}
