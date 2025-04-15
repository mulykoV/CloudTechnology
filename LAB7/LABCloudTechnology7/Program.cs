using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Azure;
using Azure.AI.TextAnalytics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    string languageKey = configuration["AzureLanguage:Key"];
    string languageEndpoint = configuration["AzureLanguage:Endpoint"];

    if (string.IsNullOrEmpty(languageKey) || string.IsNullOrEmpty(languageEndpoint))
    {
        throw new Exception("Azure Language Key або Endpoint не знайдено в конфігурації.");
    }

    var credentials = new AzureKeyCredential(languageKey);
    var endpoint = new Uri(languageEndpoint);
    return new TextAnalyticsClient(endpoint, credentials);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
