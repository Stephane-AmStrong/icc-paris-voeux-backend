using System.Linq.Expressions;
using Domain.Repositories.Abstractions;
using Domain.Shared.Common;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Persistence.Repository;

public class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    protected IMongoCollection<T> Collection { get; }

    protected RepositoryBase(IMongoDatabase database, string collectionName) => Collection = database.GetCollection<T>(collectionName);


    public Task<List<T>> BaseFindByConditionAsync(QueryParameters<T> queryParameters, CancellationToken cancellationToken)
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

        if (!string.IsNullOrWhiteSpace(queryParameters.OrderBy)) find = find.Sort(Builders<T>.Sort.Ascending(queryParameters.OrderBy));

        find = find.Skip((queryParameters.Page - 1) * queryParameters.PageSize).Limit(queryParameters.PageSize);

        return find.ToListAsync(cancellationToken);
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

    private static string GetPropertyName<TKey>(Expression<Func<T, TKey>> expression)
    {
        if (expression.Body is MemberExpression member)
            return member.Member.Name;
        if (expression.Body is UnaryExpression unary && unary.Operand is MemberExpression memberOperand)
            return memberOperand.Member.Name;
        throw new ArgumentException("Invalid orderBy expression");
    }
}
