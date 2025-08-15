#nullable enable
using System.Linq.Expressions;
using System.Reflection;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Shared.Common;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using MongoDB.Driver;

namespace Persistence.Repository;

public class RepositoryBase<T> : IRepositoryBase<T> where T : IBaseEntity
{
    protected IMongoCollection<T> Collection { get; }

    protected RepositoryBase(IMongoDatabase database, string collectionName) => Collection = database.GetCollection<T>(collectionName);


    public async Task<PagedList<T>> BaseQueryWithFiltersAsync(BaseQuery<T> queryParameters, CancellationToken cancellationToken)
    {
        List<FilterDefinition<T>> filters = [];

        // Add expression filter if any
        var filterExpression = queryParameters.GetFilterExpression();
        if (filterExpression != null) filters.Add(Builders<T>.Filter.Where(filterExpression));

        // Add text search filter if any
        if (!string.IsNullOrWhiteSpace(queryParameters.SearchTerm))
        {
            var stringProps = typeof(T).GetProperties()
                .Where(p => p.PropertyType == typeof(string));

            var regexFilters = stringProps
                .Select(p => Builders<T>.Filter.Regex(p.Name, new BsonRegularExpression(queryParameters.SearchTerm, "i")));

            if (regexFilters.Any())
            {
                var textSearchFilter = Builders<T>.Filter.Or(regexFilters);
                filters.Add(textSearchFilter);
            }
        }

        // Combine all filters with AND if more than one, otherwise Empty
        var filter = filters.Count switch
        {
            0 => Builders<T>.Filter.Empty,
            1 => filters[0],
            _ => Builders<T>.Filter.And(filters)
        };

        var find = Collection.Find(filter);

        var totalCount = await Collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var sort = BuildSortDefinition(queryParameters.OrderBy);

        if (sort != null) find = find.Sort(sort);

        find = find.Skip((queryParameters.Page - 1) * queryParameters.PageSize).Limit(queryParameters.PageSize);

        return new PagedList<T>(await find.ToListAsync(cancellationToken), totalCount, queryParameters.Page!.Value, queryParameters.PageSize!.Value);
    }

    protected async Task<List<T>> BaseGetLatestByGroupAsync<TKey>(
        Expression<Func<T, bool>> filterPredicate,
        Expression<Func<T, TKey>> groupBySelector,
        Expression<Func<T, DateTime>> orderBySelector,
        CancellationToken cancellationToken)
    {
        var filter = Builders<T>.Filter.Where(filterPredicate);

        var groupByProperty = GetPropertyName(groupBySelector);
        var orderByProperty = GetPropertyName(orderBySelector);

/*
public static BsonDocument ToBsonDocument<T>(this FilterDefinition<T> filter)
{
    var serializerRegistry = BsonSerializer.SerializerRegistry;
    var documentSerializer = serializerRegistry.GetSerializer<T>();
    return filter.Render(new RenderArgs<T>(documentSerializer, serializerRegistry));
}
*/

        var pipeline = new BsonDocument[]
        {
            // new BsonDocument("$match", filter.Render(new RenderArgs(Collection.DocumentSerializer, Collection.Settings.SerializerRegistry))),
            new BsonDocument("$match", filter.ToBsonDocument()),

            new BsonDocument("$sort", new BsonDocument
            {
                [groupByProperty] = 1,
                [orderByProperty] = -1
            }),

            new BsonDocument("$group", new BsonDocument
            {
                ["_id"] = $"${groupByProperty}",
                ["latestDocument"] = new BsonDocument("$first", "$$ROOT")
            }),

            new BsonDocument("$replaceRoot", new BsonDocument
            {
                ["newRoot"] = "$latestDocument"
            })
        };

        var cursor = await Collection.AggregateAsync<T>(pipeline, cancellationToken: cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }

    public Task<List<T>> BaseFindByConditionAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken)
    {
        return Collection.Find(expression).ToListAsync(cancellationToken);
    }

    public Task BaseCreateAsync(T entity, CancellationToken cancellationToken)
    {
        return Collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    public Task BaseUpdateAsync(T entity, CancellationToken cancellationToken)
    {
        var filter = Builders<T>.Filter.Eq(e=> e.Id, entity.Id);
        return Collection.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);
    }

    public Task BaseDeleteAsync(string id, CancellationToken cancellationToken)
    {
        var filter = Builders<T>.Filter.Eq(e=> e.Id, id);
        return Collection.DeleteOneAsync(filter, cancellationToken);
    }

    private SortDefinition<T>? BuildSortDefinition(string? orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return null;

        var orderParams = orderByQueryString.Trim().Split(',');
        var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var sortDefinitions = new List<SortDefinition<T>>();

        foreach (var param in orderParams)
        {
            var trimmedParam = param?.Trim();
            if (string.IsNullOrWhiteSpace(trimmedParam)) continue;

            var parts = trimmedParam.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) continue;

            var propertyFromQueryName = parts[0];
            var objectProperty = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

            if (objectProperty == null) continue;

            var isDescending = trimmedParam.EndsWith(" desc", StringComparison.InvariantCultureIgnoreCase);
            var sortDef = isDescending
                ? Builders<T>.Sort.Descending(objectProperty.Name)
                : Builders<T>.Sort.Ascending(objectProperty.Name);

            sortDefinitions.Add(sortDef);
        }

        return sortDefinitions.Count > 0 ? Builders<T>.Sort.Combine(sortDefinitions) : null;
    }

    private static string GetPropertyName<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyExpression)
    {
        if (propertyExpression.Body is MemberExpression memberExpression)
        {
            return memberExpression.Member.Name;
        }

        throw new ArgumentException("Expression must be a property access", nameof(propertyExpression));
    }
}
