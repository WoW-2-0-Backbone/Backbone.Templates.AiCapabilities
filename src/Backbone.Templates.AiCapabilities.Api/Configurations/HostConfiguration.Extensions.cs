using System.Reflection;
using Backbone.AiCapabilities.NaturalLanguageProcessing.ChatCompletion.OpenAi.Configurations;
using Backbone.AiCapabilities.SemanticKernel.Abstractions.OpenAi.Configurations;
using Backbone.Comms.Infra.EventBus.InMemory.Configurations;
using Backbone.Comms.Infra.Mediator.MassTransit.Configurations;
using Backbone.Comms.Infra.Mediator.MediatR.Behaviors.Configurations;
using Backbone.Comms.Infra.Mediator.MediatR.Configurations;
using Backbone.DataAccess.Relational.EfCore.Abstractions.Constants;
using Backbone.General.Serialization.Json.Newtonsoft.Configurations;
using Backbone.General.Time.Provider.Configurations;
using Backbone.General.Validations.Abstractions.Configurations;
using Backbone.General.Validations.FluentValidation.Configurations;
using Backbone.Storage.Cache.InMemory.Lazy.Configurations;
using Backbone.Templates.AiCapabilities.Application.ChatCompletions.Commands;
using Backbone.Templates.AiCapabilities.Infrastructure.Services;
using Backbone.Templates.AiCapabilities.Persistence.DataContexts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Backbone.Templates.AiCapabilities.Api.Configurations;

/// <summary>
/// Contains configurations for the host.
/// </summary>
public static partial class HostConfiguration
{
    private static Test test = new();

    private static readonly ICollection<Assembly> Assemblies = Assembly
        .GetExecutingAssembly()
        .GetReferencedAssemblies()
        .Select(Assembly.Load)
        .Append(Assembly.GetExecutingAssembly())
        .ToList();

    ///<summary>
    /// Adds general infrastructure.
    /// </summary>
    private static WebApplicationBuilder AddGeneralInfra(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddTimeProvider()
            .AddNewtonsoftJsonSerializer();

        return builder;
    }

    /// <summary>
    /// Registers logging services
    /// </summary>
    private static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
        builder.Host.UseSerilog(logger);

        return builder;
    }

    /// <summary>
    /// Adds caching services.
    /// </summary>
    private static WebApplicationBuilder AddCaching(this WebApplicationBuilder builder)
    {
        builder.Services.AddInMemoryCacheStorageWithLazyInMemoryCacheStorage(builder.Configuration);

        return builder;
    }

    /// <summary>
    /// Registers mapping services
    /// </summary>
    private static WebApplicationBuilder AddMappers(this WebApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(Assemblies);
        return builder;
    }

    /// <summary>
    /// Registers persistence infrastructure
    /// </summary>
    private static WebApplicationBuilder AddPersistence(this WebApplicationBuilder builder)
    {
        // Register db context
        builder.Services.AddDbContext<AppDbContext>((_, options) =>
        {
            options.EnableSensitiveDataLogging();
            options.UseNpgsql(builder.Configuration
                .GetConnectionString(DataAccessConstants.DefaultDatabaseConnectionString));
        });

        return builder;
    }

    /// <summary>
    /// Configures the Dependency Injection container to include validators from referenced assemblies.
    /// </summary>
    private static WebApplicationBuilder AddValidators(this WebApplicationBuilder builder)
    {
        builder.Services.AddGeneralValidationSettings(builder.Configuration);
        builder.Services.AddFluentValidation(Assemblies);

        return builder;
    }

    /// <summary>
    /// Adds communication services.
    /// </summary>
    private static WebApplicationBuilder AddInfraComms(this WebApplicationBuilder builder)
    {
        // Add a mediator pipeline with MediatR
        builder.Services
            .AddMediatRServices(Assemblies, (mediatorConfiguration, _) => mediatorConfiguration.AddMediatRPipelineBehaviors())
            .AddMediatorWithMediatR();

        // Add a mediator pipeline with MassTransit and in-memory event bus
        builder.Services
            .AddMassTransitServices(Assemblies, (config, _) => config
                .AddInMemoryEventBusWithMassTransit(builder.Services, true));

        return builder;
    }

    /// <summary>
    /// Registers semantic kernel infrastructure.
    /// </summary>
    private static WebApplicationBuilder AddAiCapabilities(this WebApplicationBuilder builder)
    {
        // Register Semantic Kernel with OpenAI
        builder.Services.AddSemanticKernelWithOpenAi(builder.Configuration, (kb, _, _) =>
        {
            // Add chat completion capability
            kb.AddOpenAiChatCompletionServices(builder.Services, builder.Configuration);
        });
        
        return builder;
    }

    /// <summary>
    /// Registers developer tools
    /// </summary>
    private static WebApplicationBuilder AddDevTools(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        return builder;
    }

    /// <summary>
    /// Registers API exposers
    /// </summary>
    private static WebApplicationBuilder AddExposers(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<ApiBehaviorOptions>(
            options => { options.SuppressModelStateInvalidFilter = true; }
        );
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services
            .AddControllers(options =>
            {
                // options.ModelBinderProviders.Insert(0, new EnumListModelBinderProvider());
            })
            .AddNewtonsoftJson();

        return builder;
    }

    /// <summary>
    /// Updates the database schema
    /// </summary>
    private static async ValueTask<WebApplication> UpdateDatabaseAsync(this WebApplication app)
    {
        var serviceScopeFactory = app.Services.GetRequiredKeyedService<IServiceScopeFactory>(null);

        // await serviceScopeFactory.MigrateAsync<AppDbContext>();

        return app;
    }

    /// <summary>
    /// Registers local file storage
    /// </summary>
    private static WebApplication UseLocalFileStorage(this WebApplication app)
    {
        app.UseStaticFiles();

        return app;
    }

    /// <summary>
    /// Registers developer tools middlewares
    /// </summary>
    private static WebApplication UseDevTools(this WebApplication app)
    {
        app.MapGet("/", () => "Hi, enjoy AI Capabilities Template API!");

        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }

    /// <summary>
    /// Registers exposer middlewares
    /// </summary>
    private static WebApplication UseExposers(this WebApplication app)
    {
        app.MapControllers();

        return app;
    }
}