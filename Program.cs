using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using dalabat.Data;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with SQL Server configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=SAMAALE\\SQLEXPRESS;Database=Dalabat;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddDbContext<DalabatDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add Controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Enable CORS for front-end integration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ensure Database & Seed Default Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DalabatDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

// Enable Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dalabat API v1");
    c.RoutePrefix = "swagger"; // Serve Swagger UI at /swagger
});

app.UseCors("AllowAll");

// Serve Restoran static files
var restoranPath = Path.Combine(builder.Environment.ContentRootPath, "Restoran");
if (Directory.Exists(restoranPath))
{
    var fileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(restoranPath);
    app.UseDefaultFiles(new DefaultFilesOptions
    {
        FileProvider = fileProvider,
        RequestPath = ""
    });
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = fileProvider,
        RequestPath = ""
    });
}
else
{
    app.UseStaticFiles();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
