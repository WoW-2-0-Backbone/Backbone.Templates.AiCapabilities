using System.Linq.Expressions;
using Backbone.Comms.Infra.Abstractions.Commands;
using Backbone.Comms.Infra.Abstractions.Queries;
using Backbone.DataAccess.Relational.EfCore.Abstractions.Extensions;
using Backbone.DataAccess.Relational.EfCore.Repositories.Cached.Repositories;
using Backbone.Storage.Cache.Abstractions.Brokers;
using Backbone.Storage.Cache.Abstractions.Models;
using Backbone.Templates.AiCapabilities.Domain.Entities;
using Backbone.Templates.AiCapabilities.Persistence.DataContexts;
using Backbone.Templates.AiCapabilities.Persistence.Extensions;
using Backbone.Templates.AiCapabilities.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Templates.AiCapabilities.Persistence.Repositories;

/// <summary>
/// Provides user repository functionality.
/// </summary>
public class UserRepository(AppDbContext dbContext, ICacheStorageBroker cacheBroker)
    : CachedPrimaryEntityRepositoryBase<User, AppDbContext>(dbContext, cacheBroker, new CacheEntryOptions()), IUserRepository
{
    public new IQueryable<User> Get(Expression<Func<User, bool>>? predicate = default, QueryOptions queryOptions = default)
    {
        return base.Get(predicate, queryOptions);
    }

    public new ValueTask<User?> GetByIdAsync(Guid userId, QueryOptions queryOptions = default, CancellationToken cancellationToken = default)
    {
        return base.GetByIdAsync(userId, queryOptions, cancellationToken);
    }

    public async ValueTask<User?> GetByEmailAddressAsync(string emailAddress, QueryOptions queryOptions = default,
        CancellationToken cancellationToken = default)
    {
        // Query from local entities snapshot storage.
        var user = DbContext.Set<User>().Local.FirstOrDefault(user => user.EmailAddress == emailAddress);
        if (user is not null)
            return user;

        // Query from cache storage.
        user = await CacheStorageBroker.GetOrSetAsync(
            key: CacheKeyExtensions.GetByEmailAddress<User>(emailAddress),
            entryOptions: default,
            valueProvider: async ct => await Get(existingUser => existingUser.EmailAddress == emailAddress, queryOptions).FirstOrDefaultAsync(ct),
            onCacheHit: entry =>
            {
                if (entry is not null)
                    DbContext.Entry(entry).ApplyTrackingMode(queryOptions.TrackingMode);
            },
            cancellationToken: cancellationToken);

        return user;
    }

    public new ValueTask<bool> CheckAsync<TValue>(IQueryable<TValue> queryableSource, TValue? expectedValue = default,
        CancellationToken cancellationToken = default)
    {
        return base.CheckAsync(queryableSource, expectedValue, cancellationToken);
    }

    public new ValueTask<User> CreateAsync(User user, CommandOptions commandOptions = default, CancellationToken cancellationToken = default)
    {
        return base.CreateAsync(user, commandOptions, cancellationToken);
    }

    public new ValueTask<User> UpdateAsync(User user, CommandOptions commandOptions = default, CancellationToken cancellationToken = default)
    {
        return base.UpdateAsync(user, commandOptions, cancellationToken);
    }

    public new ValueTask<User?> DeleteByIdAsync(Guid userId, CommandOptions commandOptions = default,
        CancellationToken cancellationToken = default)
    {
        return base.DeleteByIdAsync(userId, commandOptions, cancellationToken);
    }

    public new ValueTask<User?> DeleteAsync(User user, CommandOptions commandOptions = default, CancellationToken cancellationToken = default)
    {
        return base.DeleteAsync(user, commandOptions, cancellationToken);
    }
}