namespace Backbone.Templates.AiCapabilities.Persistence.Extensions;

/// <summary>
/// Contains extensions for cache key generation
/// </summary>
public static class CacheKeyExtensions
{
    #region General cache keys

    /// <summary>
    /// Generates a cache key for check entity existence by ID operation
    /// </summary>
    /// <typeparam name="TModel">The type of the model for which the cache key is being generated.</typeparam>
    /// <param name="id">The ID of the entity</param>
    /// <returns>Generated cache key</returns>
    public static string CheckById<TModel>(Guid id) => $"{typeof(TModel).Name}-{nameof(CheckById)}-{id}";

    /// <summary>
    /// Generates a cache key for getting a model with related email address.
    /// </summary>
    /// <typeparam name="TModel">The type of the model for which the cache key is being generated.</typeparam>
    /// <param name="emailAddress">The email address related to entity.</param>
    /// <returns>A string that represents the generated cache key based on the entity ID.</returns>
    public static string GetByEmailAddress<TModel>(string emailAddress) => $"{typeof(TModel).Name}-{nameof(GetByEmailAddress)}-{emailAddress}";

    #endregion
}