using System.Reflection;
using System.Text.Json.Serialization;
using FileStorage.Api.Middlewares;
using FileStorage.Application.Interfaces;
using FileStorage.Infrastructure.Common.Extensions;
using FileStorage.Infrastructure.Data;
using FileStorage.Infrastructure.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();
builder.Configuration.AddEnvironmentVariables();

builder.WebHost.UseKestrel(options =>
{
    // максимальный размер тела запроса - из конфигурации в клиобайтах (ключ MaxFileSizeKilobytes)
    options.Limits.MaxRequestBodySize = builder.Configuration.GetValue<int>("MaxFileSizeKilobytes") * 1024;
});

builder.Services.AddFileStorageDbContext(builder.Configuration);

builder.Services.AddControllers().AddJsonOptions(o =>
{
    // отключаем циклическую зависимость при сераиализации
    o.JsonSerializerOptions
        .ReferenceHandler = ReferenceHandler.Preserve;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "FileStorage", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IFileService, FileService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<CustomExceptionsHandler>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetService<FileStorageDbContext>();
    dbContext!.Database.EnsureCreated();
}

app.Run();