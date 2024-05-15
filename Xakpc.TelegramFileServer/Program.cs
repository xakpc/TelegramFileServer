using Microsoft.OpenApi.Models;
using Xakpc.TelegramFileServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IStorageService, TelegramStorageService>();

builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Telegram File Server API",
        Version = "v1",
        Description = "REST-based file server API that uses Telegram Bot as storage. " +
                      "[How to use in the README](https://github.com/xakpc/TelegramFileServer?tab=readme-ov-file#how-to-use).",
        Contact = new OpenApiContact
        {
            Name = "@xakpc",
            Url = new Uri("https://x.com/xakpc")
        }
    });
});
builder.Services.AddProblemDetails();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(s =>
{
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "Telegram File Server API");
    s.RoutePrefix = "";
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
