using System;
using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Extensions.Configuration;

namespace LAB7
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string languageKey = configuration["AzureLanguage:Key"];
            string languageEndpoint = configuration["AzureLanguage:Endpoint"];

            if (string.IsNullOrEmpty(languageKey) || string.IsNullOrEmpty(languageEndpoint))
            {
                Console.WriteLine("Azure Language Key або Endpoint не знайдено в конфігурації.");
                return;
            }

            var credentials = new AzureKeyCredential(languageKey);
            var endpoint = new Uri(languageEndpoint);
            var client = new TextAnalyticsClient(endpoint, credentials);

            RecognizePIIExample(client);

            Console.Write("Press any key to exit.");
            Console.ReadKey();
        }

        static void RecognizePIIExample(TextAnalyticsClient client)
        {
            string document = "Call our office at 312-555-1234, or send an email to support@contoso.com.";

            PiiEntityCollection entities = client.RecognizePiiEntities(document).Value;

            Console.WriteLine($"Redacted Text: {entities.RedactedText}");
            if (entities.Count > 0)
            {
                Console.WriteLine($"Recognized {entities.Count} PII entit{(entities.Count > 1 ? "ies" : "y")}:");
                foreach (PiiEntity entity in entities)
                {
                    Console.WriteLine($"Text: {entity.Text}, Category: {entity.Category}, SubCategory: {entity.SubCategory}, Confidence score: {entity.ConfidenceScore}");
                }
            }
            else
            {
                Console.WriteLine("No entities were found.");
            }
        }
    }
}