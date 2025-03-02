using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.AI.TextAnalytics;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace LAB4
{
    public class Function1
    {
        private readonly ILogger _logger;
        private readonly TextAnalyticsClient _textAnalyticsClient;
        private readonly BlobServiceClient _blobServiceClient;

        public Function1(ILoggerFactory loggerFactory, TextAnalyticsClient textAnalyticsClient, BlobServiceClient blobServiceClient)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
            _textAnalyticsClient = textAnalyticsClient;
            _blobServiceClient = blobServiceClient;
        }

        [Function("Function1")]
        public async Task Run(
    [BlobTrigger("source/{name}", Connection = "blobConn")] byte[] myBlob, string name)
        {
            using (var memoryStream = new MemoryStream(myBlob))
            using (var reader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                string content = await reader.ReadToEndAsync();
                _logger.LogInformation($"C# Blob Trigger Processed blob\n Name: {name} \n Data: {content}");

                var detectedLanguage = await _textAnalyticsClient.DetectLanguageAsync(content);
                string languageName = detectedLanguage.Value.Name;
                _logger.LogInformation($"Detected language: {languageName}");

                string targetBlobName = $"{languageName}/{name}";
                BlobContainerClient targetContainer = _blobServiceClient.GetBlobContainerClient(Environment.GetEnvironmentVariable("targetContainerName"));
                BlobClient targetBlob = targetContainer.GetBlobClient(targetBlobName);

                using (var uploadStream = new MemoryStream(myBlob))
                {
                    await targetBlob.UploadAsync(uploadStream);
                }

                _logger.LogInformation($"Uploaded blob to {targetBlobName} in {Environment.GetEnvironmentVariable("targetContainerName")} container.");
            }
        }


    }
}