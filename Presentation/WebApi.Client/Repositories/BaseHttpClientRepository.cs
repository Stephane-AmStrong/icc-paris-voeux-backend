using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using WebApi.Client.DataTransferObjects;
using WebApi.Client.Paging;

namespace WebApi.Client.Repositories;

public abstract class BaseHttpClientRepository(
    HttpClient httpClient,
    ILogger logger,
    string endpoint,
    string entityName = "entity")
{
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    
    private readonly Uri _endpointUri = new(httpClient.BaseAddress ?? throw new ArgumentException("HttpClient must have BaseAddress"), endpoint);
    
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private string EntityName { get; } = entityName;

    // CREATE
    public async Task<TResponse> BaseCreateAsync<TRequest, TResponse>(TRequest createRequest, CancellationToken cancellationToken) where TRequest : class where TResponse : IBaseDto
    {
        if (createRequest == null) throw new ArgumentNullException(nameof(createRequest));

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, _endpointUri);
            request.Content = JsonContent.Create(createRequest, options: _options);

            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            await HandleErrorResponseAsync(response, $"creating {EntityName}", cancellationToken);

            await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var createdEntity = await JsonSerializer.DeserializeAsync<TResponse>(contentStream, _options, cancellationToken);

            _logger.LogInformation("Successfully created {EntityName} with ID {EntityId}", EntityName, createdEntity?.Id);
            return createdEntity;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while creating {EntityName}", EntityName);
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON serialization error while creating {EntityName}", EntityName);
            throw;
        }
    }

    // READ (Get by ID)
    public async Task<TResponse> BaseGetByIdAsync<TResponse>(string entityId, CancellationToken cancellationToken) where TResponse : IBaseDto
    {
        if (string.IsNullOrWhiteSpace(entityId)) throw new ArgumentException($"{EntityName} ID cannot be null or empty", nameof(entityId));

        var getByIdUri = new Uri(_endpointUri, entityId);

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, getByIdUri);
            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            await HandleErrorResponseAsync(response, $"retrieving {EntityName} {entityId}", cancellationToken);

            await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var entity = await JsonSerializer.DeserializeAsync<TResponse>(contentStream, _options, cancellationToken);

            _logger.LogInformation("Successfully retrieved {EntityName} with ID {EntityId}", EntityName, entityId);
            return entity;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while retrieving {EntityName} {EntityId}", EntityName, entityId);
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error while retrieving {EntityName} {EntityId}", EntityName, entityId);
            throw;
        }
    }

    // READ (Get all with query)
    public async Task<PagedList<TResponse>> BaseGetPagedListAsync<TResponse>(QueryParameters query, CancellationToken cancellationToken) where TResponse : IBaseDto
    {
        try
        {
            var queryString = BuildQueryString(query);
            var endpoint = string.IsNullOrEmpty(queryString) ? _endpointUri : new Uri(_endpointUri, queryString);

            using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            await HandleErrorResponseAsync(response, $"retrieving {EntityName}s", cancellationToken);

            using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var entities = await JsonSerializer.DeserializeAsync<PagedList<TResponse>>(contentStream, _options, cancellationToken);

            _logger.LogInformation("Successfully retrieved {EntityCount} {EntityName}s", entities?.MetaData.TotalCount ?? 0, EntityName);
            return entities;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while retrieving {EntityName}s", EntityName);
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error while retrieving {EntityName}s", EntityName);
            throw;
        }
    }

    // UPDATE
    public async Task BaseUpdateAsync<TRequest>(string entityId, TRequest updateRequest, CancellationToken cancellationToken) where TRequest : class
    {
        if (string.IsNullOrWhiteSpace(entityId)) throw new ArgumentException($"{EntityName} ID cannot be null or empty", nameof(entityId));

        ArgumentNullException.ThrowIfNull(updateRequest);

        var updateUri = new Uri(_endpointUri, entityId);

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Put, updateUri);
            request.Content = JsonContent.Create(updateRequest, options: _options);

            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            await HandleErrorResponseAsync(response, $"updating {EntityName} {entityId}", cancellationToken);

            _logger.LogInformation("Successfully updated {EntityName} with ID {EntityId}", EntityName, entityId);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while updating {EntityName} {EntityId}", EntityName, entityId);
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON serialization error while updating {EntityName} {EntityId}", EntityName, entityId);
            throw;
        }
    }

    // DELETE
    public async Task BaseDeleteAsync(string entityId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(entityId)) throw new ArgumentException($"{EntityName} ID cannot be null or empty", nameof(entityId));

        var deleteUri = new Uri(_endpointUri, entityId);

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, deleteUri);
            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            await HandleErrorResponseAsync(response, $"deleting {EntityName} {entityId}", cancellationToken);

            _logger.LogInformation("Successfully deleted {EntityName} with ID {EntityId}", EntityName, entityId);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while deleting {EntityName} {EntityId}", EntityName, entityId);
            throw;
        }
    }

    private async Task HandleErrorResponseAsync(HttpResponseMessage response, string operation, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode) return;

        var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var headers = string.Join(", ", response.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"));

        _logger.LogError("HTTP {StatusCode} error while {Operation}. Headers: {Headers}. Content: {ErrorContent}",
            (int)response.StatusCode, operation, headers, errorContent);

        throw response.StatusCode switch
        {
            System.Net.HttpStatusCode.BadRequest => new ArgumentException($"Invalid request: {errorContent}"),
            System.Net.HttpStatusCode.Unauthorized => new UnauthorizedAccessException($"Authentication failed: {errorContent}"),
            System.Net.HttpStatusCode.NotFound => new InvalidOperationException($"Endpoint not found: {errorContent}"),
            System.Net.HttpStatusCode.InternalServerError => new InvalidOperationException($"Server error: {errorContent}"),
            _ => new HttpRequestException($"HTTP {(int)response.StatusCode} {response.StatusCode}: {errorContent}")
        };
    }

    private string BuildQueryString(QueryParameters query)
    {
        if (query == null) return string.Empty;

        var queryParams = new List<KeyValuePair<string, StringValues>>();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            queryParams.Add(KeyValuePair.Create(nameof(query.SearchTerm), new StringValues(query.SearchTerm)));

        if (!string.IsNullOrWhiteSpace(query.OrderBy))
            queryParams.Add(KeyValuePair.Create(nameof(query.OrderBy), new StringValues(query.OrderBy)));

        if (query.Page is not null && query.Page > 1)
            queryParams.Add(KeyValuePair.Create(nameof(query.Page), new StringValues(query.Page.ToString())));

        if (query.PageSize is not null && query.PageSize != 10)
            queryParams.Add(KeyValuePair.Create(nameof(query.PageSize), new StringValues(query.PageSize.ToString())));

        var specificParams = AddSpecificQueryParameters(query);
        if (specificParams?.Count > 0)
        {
            queryParams.AddRange(specificParams);
        }

        return queryParams.Count == 0 ? string.Empty :
               QueryString.Create(queryParams).ToString().TrimStart('?');
    }

    protected virtual List<KeyValuePair<string, StringValues>> AddSpecificQueryParameters(QueryParameters query)
    {
        return new List<KeyValuePair<string, StringValues>>();
    }
}
