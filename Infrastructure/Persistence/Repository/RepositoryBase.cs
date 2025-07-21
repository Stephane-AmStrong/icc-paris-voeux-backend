using System.Linq.Expressions;
using System.Reflection;
using Domain.Repositories.Abstractions;
using Domain.Shared.Common;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Persistence.Repository;

public class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    protected IMongoCollection<T> Collection { get; }

    protected RepositoryBase(IMongoDatabase database, string collectionName) => Collection = database.GetCollection<T>(collectionName);


    public async Task<PagedList<T>> BaseQueryWithFiltersAsync(QueryParameters<T> queryParameters, CancellationToken cancellationToken)
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

            filters.AddRange(regexFilters);
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

        int page = Math.Max(queryParameters.Page ?? 1, 1);
        int pageSize = Math.Max(queryParameters.PageSize ?? 10, 10);
        find = find.Skip((page - 1) * pageSize).Limit(pageSize);

        return new PagedList<T>(await find.ToListAsync(cancellationToken), totalCount, page, pageSize);
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
        var idProperty = typeof(T).GetProperty("Id") ?? throw new InvalidOperationException("The entity must have a property 'Id'.");
        var idValue = idProperty.GetValue(entity);
        var filter = Builders<T>.Filter.Eq("Id", idValue);

        return Collection.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);
    }

    public Task BaseDeleteAsync(T entity, CancellationToken cancellationToken)
    {
        var idProperty = typeof(T).GetProperty("Id") ?? throw new InvalidOperationException("The entity must have a property 'Id'.");
        var idValue = idProperty.GetValue(entity);
        var filter = Builders<T>.Filter.Eq("Id", idValue);

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
}
