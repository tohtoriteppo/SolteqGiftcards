using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

string baseUrl = builder.Configuration["Api:StageAPIbaseUrl"] ?? throw new Exception("Missing API base URL");

builder.Services.AddHttpClient<AuthService>();
builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddTransient<CsvExporter>();

var host = builder.Build();

AuthService auth = host.Services.GetRequiredService<AuthService>();
ApiService api = host.Services.GetRequiredService<ApiService>();
CsvExporter exporter = host.Services.GetRequiredService<CsvExporter>();

Console.WriteLine("Getting authentication token...");
string token = await auth.GetTokenAsync();
Console.WriteLine("Token received.");

Console.WriteLine("Retrieving gift cards...");
string json = await api.GetDataAsync(token);
Console.WriteLine($"Gift cards received.");

Console.WriteLine("Exporting CSV...");
await exporter.ExportAsync(json, "export.csv");
Console.WriteLine("Done!");