using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Extensions.Configuration;

namespace Example
{
    class Program
    {
        static async Task Main(string[] args)
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

            await HealthExample(client);
        }

        static async Task HealthExample(TextAnalyticsClient client)
        {
            string document = "Prescribed 100mg ibuprofen, taken twice daily.";

            List<string> batchInput = new List<string>() { document };
            AnalyzeHealthcareEntitiesOperation healthOperation = await client.StartAnalyzeHealthcareEntitiesAsync(batchInput);
            await healthOperation.WaitForCompletionAsync();

            await foreach (AnalyzeHealthcareEntitiesResultCollection documentsInPage in healthOperation.Value)
            {
                Console.WriteLine($"Results of Azure Text Analytics for health async model, version: \"{documentsInPage.ModelVersion}\"");
                Console.WriteLine("");

                foreach (AnalyzeHealthcareEntitiesResult entitiesInDoc in documentsInPage)
                {
                    if (!entitiesInDoc.HasError)
                    {
                        foreach (var entity in entitiesInDoc.Entities)
                        {
                            Console.WriteLine($"  Entity: {entity.Text}");
                            Console.WriteLine($"  Category: {entity.Category}");
                            Console.WriteLine($"  Offset: {entity.Offset}");
                            Console.WriteLine($"  Length: {entity.Length}");
                            Console.WriteLine($"  NormalizedText: {entity.NormalizedText}");
                        }
                        Console.WriteLine($"  Found {entitiesInDoc.EntityRelations.Count} relations in the current document:");
                        Console.WriteLine("");

                        foreach (HealthcareEntityRelation relations in entitiesInDoc.EntityRelations)
                        {
                            Console.WriteLine($"    Relation: {relations.RelationType}");
                            Console.WriteLine($"    For this relation there are {relations.Roles.Count} roles");

                            foreach (HealthcareEntityRelationRole role in relations.Roles)
                            {
                                Console.WriteLine($"      Role Name: {role.Name}");
                                Console.WriteLine($"      Associated Entity Text: {role.Entity.Text}");
                                Console.WriteLine($"      Associated Entity Category: {role.Entity.Category}");
                                Console.WriteLine("");
                            }
                            Console.WriteLine("");
                        }
                    }
                    else
                    {
                        Console.WriteLine("  Error!");
                        Console.WriteLine($"  Document error code: {entitiesInDoc.Error.ErrorCode}.");
                        Console.WriteLine($"  Message: {entitiesInDoc.Error.Message}");
                    }
                    Console.WriteLine("");
                }
            }
        }
    }
}