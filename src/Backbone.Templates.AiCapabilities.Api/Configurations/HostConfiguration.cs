namespace Backbone.Templates.AiCapabilities.Api.Configurations;

/// <summary>
/// Contains configurations for the host.
/// </summary>
public static partial class HostConfiguration
{
    /// <summary>
    /// Configures application builder
    /// </summary>
    public static ValueTask<WebApplicationBuilder> ConfigureAsync(this WebApplicationBuilder builder)
    {
        builder
            .AddGeneralInfra()
            .AddLogging()
            .AddCaching()
            .AddMappers()
            .AddPersistence()
            .AddValidators()
            .AddInfraComms()
            .AddAiCapabilities()
            .AddDevTools()
            .AddExposers();
            
        return new(builder);
    }

    /// <summary>
    /// Configures application
    /// </summary>
    public static async ValueTask<WebApplication> ConfigureAsync(this WebApplication app)
    {
        // await app.MigrateDataBaseSchemasAsync();
        
        app
            .UseLocalFileStorage()
            .UseDevTools()
            .UseExposers();

        return app;
    }
}