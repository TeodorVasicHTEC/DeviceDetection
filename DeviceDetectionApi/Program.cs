using Wangkanai.Detection.Models;
using Wangkanai.Detection.Services;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDetection();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDetection();
//app.UseHttpsRedirection();

const string appleStoreLink = "https://apps.apple.com/ph/app/apple-music-classical/id1598433714";
const string playStoreLink = @"http://market.android.com/details?id=com.imangi.templerun&hl=en";
const string unsupportedDevice = "https://daouvineyards.com/unsupported";

app.MapGet("/downloadAppCustom", (HttpContext context) =>
    {
        var agent = context.Request.Headers.UserAgent.ToString();

        Console.WriteLine(agent.ToString());

        if (agent.Contains("iPhone") || agent.Contains("Macintosh") || agent.Contains("iPad"))
        {
            // It is an IOS device, redirect to the specified location here
            return Results.Redirect(appleStoreLink, true, true);
        }

        if (agent.Contains("Android") || agent.Contains("Linux"))
        {
            // It is an Android device, redirect to the specified location here
            return Results.Redirect(playStoreLink, true, true);
        }

        return Results.Redirect(unsupportedDevice, true, true);
    })
.WithName("DownloadApplicationCustom");

app.MapGet("/downloadAppNugetPackage", (IDetectionService detectionService) =>
{
    Console.WriteLine("Here library work for us.");
    switch (detectionService.Platform.Name)
        {
            case Platform.iOS:
            case Platform.iPadOS:
                return Results.Redirect(appleStoreLink, true, true);
                break;
            case Platform.Android:
                return Results.Redirect(playStoreLink, true, true);
            break;
            default:
                return Results.Redirect(unsupportedDevice, true, true);
    }
})
.WithName("DownloadAppNugetPackage");

app.Run();