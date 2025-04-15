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
                    content = @"� ������ �������� ���� �� ����� �������. � �� ���� � ����� �� ���� ���� ���� ���������, � ���������� ������, ��, �������, ������� ����� ���� ��. ���, � ������ ������, ����� ��� ������. � ����� �� ����� �������, ����� ��� ����� �������� ��, ������, �� �������� ����, �� ������ �������� �� ������� ���� ������. ��� ����� �� ������� ���������� �������� ����� ���� �� ��� ������� � ���� ������, ��������, ������, �� ��� � ������� ������, �� ������ �������� � � ����� ����. ������� �� ������� � ��������� ������ �������� ��� ���. �� ������� ������ ������ ���, ���������� ����, a ��� ���� ������� ��� ������ �������, �� ����� ���� � ��������� ������� ��� ����� �� ����, � ��� ����� � � ��� �����. ��� �����, �� � ��� ������, ��� ���� ������� ���� ������, ��������. � �����, ��������� ���� ���, �������� ����������. �! ����� ����������!.. A ����� �� �����. � ���� ��� � ���� ������� ����. � �� ��� �� ������. ����? � � ��� ���� ���, ���� ���� �������, �� ��� ����� �� ����; ����, �� ������ �� ������ �� �����, �� ���� �������� ������ ���� � ������ ������. ��� ����, �������� ���� ����, ����� ����� ������� ���������� ��� ����� � ���� ������� ����. ���� �������� �����, ��������� �� ��� ��� �����. ���� � ������ �����!..";
                    break;
                case "intermezzo":
                    title = "Intermezzo";
                    content = @"�������� ����� �� �����������... �� ���� ���� � ��� ������������ ""�����"", �� ���� ��� ������� � �� ������ �����. �����, �� �� ""�����"" ����, �� ������, � ���� �� ��, �� ����� ��� ���� ������ �����, �� �� � ��, � ���� ���� ��� ����. �������� ���� ���������� ����� ������������� ����. ��� �� ��� ���������� �� �����, ������, �������. � ��������.

�� ����� ��������� � ����������� ��� �� ����, �� ����� �� �����. �� ����� ������, � � ����. � ����� � ���� � ����, �� �������� ������ �����, � ���� ����������? � ���, �� ���� ��������� ������� � ��, ��� ������ ���� ���� � ����, �� ���� ������� � ����. � �� ���� ����������� � �������. � �� ���� ����, �������. ��������� � ������ ��������: ���� ����� ��� �����, � ��� �� ��� �� �� ���� �����. ��� �� �� ���� � ����� � ������ ������ ������.

���, �� ���� ��� �� ����� � ������, �� ��� �� ���� �����. �� �����. �� �� ������ ����� � ����� � �����, �� �� ����� ���� ������� � ������ ������ ���� � ���� ����� ��������. �� ����� ����� ���� ���� ��������� ������, ������ ����, ������� ������ ����� �� �����, ����� �� ����, � ������, �����. �� ������. ����� � ������ ��� ������; ��� ���, �����, ����, ������� � ����, � ���� ��, � ���� ��������� ������� � ����, ��������� � ��� �����. � �� ���� ����������� � �����... � �� ���� ���� �������... �� �� ����� ���� ����� �� ����, �� ������ ��������� � ����. �� ����� � �� �����, �� �� �������� ������, ��� ���������� � ��� ���, ������ ��䳿 � ���� ������. ���� ���������� � ������ ���������. ���� ���, ���� ���� ����� ���������. ��� ��� ���, �� �� ���� �����? �� ����� ���� ��� �����, ����� ����� ����... �� ����, �� �����, ��� ���� � �� �����... �� ����� ������� ����, ��� ��� ����, �� ��� �����. � �� �� �����. � ���� �� ���, �� ����, � �� �� ��� ����� � ���� ���������� ""�����"", � ����������� ""�����"".
";
                    break;
                case "thetale":
                    title = "The Tale by Stephen King";
                    content = @"I was seventeen and knew nothing. Life was a movie I�d just walked into, late. The plot was unclear, the characters were confusing, and I was sure I�d missed something important.
My father had died two years before, and I still hadn�t figured out how to fill the space he left behind. Grief is like a room you keep going back to, even when you know there�s nothing new inside.""
Then one day, I found the key. Not a metaphorical one, but a real, heavy, old-fashioned key in the pocket of a man who shouldn�t have had it. That was the day everything changed � the day I discovered the door, and what was waiting behind it.";
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
