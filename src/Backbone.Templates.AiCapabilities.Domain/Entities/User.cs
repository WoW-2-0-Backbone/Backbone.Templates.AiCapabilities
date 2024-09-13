using Backbone.DataAccess.Relational.Entities.Models;
using Backbone.Storage.Cache.Abstractions.Models;

namespace Backbone.Templates.AiCapabilities.Domain.Entities;

/// <summary>
/// Represents user in the system.
/// </summary>
public class User : IPrimaryEntity, ICacheEntry
{
    /// <inheritdoc />
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user email address.
    /// </summary>
    public string EmailAddress { get; set; } = default!;

    /// <inheritdoc />
    public string CacheKey => Id.ToString();
}