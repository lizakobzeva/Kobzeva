using gymBackendC__.WebApi;
using gymBackendC__.WebApi.Middleware;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// -- logger
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();

// -- swagger
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerConfiguration(builder.Configuration);
}

// -- auth
builder.Services.AddHybridAuthentication(builder.Configuration);
builder.Services.AddHybridAuthorization();
builder.Services.AddAuthHelpers(builder.Configuration);

// -- controllers
builder.Services.AddCustomHealthChecks(builder.Configuration);
builder.Services.RegisterControllers();
builder.Services.AddValidators();
builder.Services.AddCorsPolicy();

// -- services
builder.Services.RegisterServices();
builder.Services.AddMappings();
builder.Services.AddRedisCache(builder.Configuration);

// -- repositories
builder.Services.RegisterRepositories();

// -- data
builder.Services.AddDbContextPoolWithPostgres(builder.Configuration.GetConnectionString("Postgres"));


var app = builder.Build();

// -- middlewares
app.MapHealthChecks("/health", new HealthCheckOptions { ResponseWriter = Init.WriteHealthChecksResponse });
app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.RunAsync();
