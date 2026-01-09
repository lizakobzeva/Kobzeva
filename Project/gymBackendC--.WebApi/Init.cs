using System.Data.Common;
using System.Text;
using gymBackendC__.WebApi.Auth;
using gymBackendC__.WebApi.Data;
using gymBackendC__.WebApi.Mappings;
using gymBackendC__.WebApi.Repositories;
using gymBackendC__.WebApi.Services;
using gymBackendC__.WebApi.Validators;

using FluentValidation;
using gymBackendC__.WebApi.Middleware;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;

namespace gymBackendC__.WebApi;

public static partial class Init
{
    public static IServiceCollection AddSwaggerConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var swaggerSecurityDefinition = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token below. Example: Bearer {token}"
        };
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", swaggerSecurityDefinition);
            options.OperationFilter<SwaggerSecurityOperationFilter>();
        });
        return services;
    }
    
    public static IServiceCollection AddHybridAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                throw new UnauthorizedAccessException("Authentication failed");
            },
            OnChallenge = context =>
            {
                throw new UnauthorizedAccessException("Authentication failed");
            },
            OnForbidden = context =>
            {
                throw new SecurityTokenException("Authorization failed");
            }
        };

        var tokenValidationParams = new TokenValidationParameters
        {
            RoleClaimType = "role",
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
        };

        Func<HttpContext,string> authTypeSelector = context =>
        {
            if (context.Request.Headers.ContainsKey("Authorization"))
                return "Jwt";
            if (context.Request.Headers.ContainsKey("X-API-KEY"))
                return "ApiKey";
            return "Jwt"; // default fallback
        };
        
        services
            .AddAuthentication(options => options.DefaultScheme = "Hybrid")
            .AddJwtBearer("Jwt", options =>
            {
                options.Events = events;
                options.TokenValidationParameters = tokenValidationParams;
            })
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthScheme>("ApiKey", null)
            .AddPolicyScheme("Hybrid", "Hybrid JWT or API Key", options =>
            {
                options.ForwardDefaultSelector = authTypeSelector;
            });
        return services;
    }

    public static IServiceCollection AddHybridAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("CanRead", policy =>
                policy.RequireClaim("permissions", Permissions.Read));

            options.AddPolicy("CanCreate", policy =>
                policy.RequireClaim("permissions", Permissions.Create));

            options.AddPolicy("CanUpdate", policy =>
                policy.RequireClaim("permissions", Permissions.Update));

            options.AddPolicy("CanDelete", policy =>
                policy.RequireClaim("permissions", Permissions.Delete));
        });
        return services;
    }

    public static IServiceCollection AddAuthHelpers(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.Configure<JWTConfiguration>(configuration.GetSection("Jwt"));
        services.AddScoped<ICurrentUser, HttpCurrentUser>();
        services.AddScoped<IJWTHelper, JWTHelper>();
        return services;
    }
    
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options => 
        {
            options.AddDefaultPolicy(corsBuilder => 
            {
                corsBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
        });
        return services;
    }
    
    public static IServiceCollection AddCustomHealthChecks(
        this IServiceCollection services, 
        IConfiguration configuration
    )
    {
        services.AddHealthChecks()
            .AddNpgSql(
                configuration.GetConnectionString("Postgres")!,
                name: "postgresql",
                failureStatus: HealthStatus.Unhealthy
            )
            .AddRedis(
                configuration.GetConnectionString("Redis")!,
                name: "redis",
                failureStatus: HealthStatus.Unhealthy
            );
        return services;
    }

    public static async Task WriteHealthChecksResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
        };
        await context.Response.WriteAsJsonAsync(result);
    }
    
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<TrainingsRequestModelValidator>();
        services.AddValidatorsFromAssemblyContaining<ExercisesRequestModelValidator>();
        services.AddValidatorsFromAssemblyContaining<LoginRequestModelValidator>();
        services.AddValidatorsFromAssemblyContaining<RegisterRequestModelValidator>();
        return services;
    }
    
    public static IServiceCollection AddMappings(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile<TrainingsMappingProfile>());
        services.AddAutoMapper(cfg => cfg.AddProfile<ExercisesMappingProfile>());
        return services;
    }
    
    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "cachingRequests:";
        });
        return services;
    }
    
    public static IServiceCollection AddDbContextPoolWithPostgres(this IServiceCollection services, string? connectionString)
    {
        services.AddDbContextPool<AppDbContext>(options =>
            options.UseNpgsql(connectionString)
        );
        services.AddScoped<DbConnection>(_ =>
            new NpgsqlConnection(connectionString)
        );
        return services;
    }
    
    public static IServiceCollection RegisterControllers(this IServiceCollection services)
    {
        services.
            AddControllers(options => { options.Filters.Add<ValidationFilter>(); })
            // выключение ошибок валидации по умолчанию
            .ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = true; });
        return services;
    }
    
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<ITrainingsService, TrainingsService>();
        services.AddScoped<IExercisesService, ExercisesService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
    
    public static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITrainingsRepository, TrainingsRepository>();
        services.AddScoped<IExercisesRepository, ExercisesRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        return services;
    }
}