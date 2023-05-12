using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using System.Dynamic;
using System.Net.Http;
using System.Text.Json;

const string OPEN_MATEO_BASEURL = "https://api.open-meteo.com/v1/forecast?";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("https://chat.openai.com", "http://localhost:5100").AllowAnyHeader().AllowAnyMethod();
    });
});


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ChatGPT.Weather",
        Version = "v1",
        Description = "A plugin that allows you to get current weather information for a given city"
    });
});

var app = builder.Build();
app.UseCors("AllowAll");
// Configure the HTTP request pipeline.

app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
    {
        swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } };
    });
});
app.UseSwaggerUI(x =>
{
    x.SwaggerEndpoint("/swagger/v1/swagger.yaml", "ChatGPT.Weather v1");

});



app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), ".well-known")),
    RequestPath = "/.well-known"
});

/// <summary>
/// Gets the weather for a latitude and longitude
/// </summary>
app.MapGet("/weather", async (double latitude, double longitude) =>
{
    using (var httpClient = new HttpClient())
    {
        var queryParams = $"latitude={latitude}&longitude={longitude}&hourly=temperature_2m&current_weather=true";
        var url = OPEN_MATEO_BASEURL + queryParams;
        var result = await httpClient.GetStringAsync(url);
        var jsonDocument = JsonDocument.Parse(result);
        var currentWeatherElement = jsonDocument.RootElement.GetProperty("current_weather");
        return JsonSerializer.Deserialize<GetWeatherResponse>(currentWeatherElement);

    }
})
.WithName("GetWeatherForecast")
.WithOpenApi(generatedOperation =>
{
    generatedOperation.Description = "Gets the current weather for a city by latitude and longitude";
    var parameter = generatedOperation.Parameters[0];
    parameter.Description = "The latitude of the city to retrieve the current weather";

    parameter = generatedOperation.Parameters[1];
    parameter.Description = "The longitude of the city to retrieve the current weather";

    return generatedOperation;
});





app.Run();

public partial class Program { }
internal class GetWeatherResponse
{
    public double temperature { get; set; }
    public double windspeed { get; set; }
    public double winddirection { get; set; }

    public GetWeatherResponse() { }
    public GetWeatherResponse(double temperature, double windspeed, double winddirection)
    {
        this.temperature = temperature;
        this.windspeed = windspeed;
        this.winddirection = winddirection;
    }
}



