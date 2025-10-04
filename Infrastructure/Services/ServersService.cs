#nullable enable
using Application.Abstractions.Services;
using Application.UseCases.ServerStatuses.GetByQuery;
using Application.UseCases.Servers.GetByQuery;
using DataTransfertObjects.QueryParameters;
using DataTransfertObjects.Requests;
using DataTransfertObjects.Responses;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Errors;
using Domain.Shared;
using Domain.Shared.Common;
using Mapster;



using Microsoft.Extensions.Logging;

namespace Services;

public sealed class ServersService(IServersRepository serversRepository, IServerStatusesRepository serverStatusesRepository, ILogger<ServersService> logger) : IServersService
{
    public async Task<PagedList<ServerResponse>> GetPagedListByQueryAsync(ServerQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting servers with query parameters: {@QueryParameters}", query);
        var servers = await serversRepository.GetPagedListByQueryAsync(query, cancellationToken);

        var serverIds = servers.Select(server => server.Id).ToArray();
        var serverStatuses = await serverStatusesRepository.GetLatestByServerIdsAsync(serverIds, cancellationToken);

        logger.LogDebug("Found {ServerStatusCount} serverStatuses for {ServerIdCount} server IDs", serverStatuses.Count, serverIds.Length);
        var serverStatusesByServerId = serverStatuses.ToDictionary(p => p.ServerId, p => p.Adapt<ServerStatusResponse>());

        var serverResponses = servers.Select(server =>
        {
            var serverResponse = server.Adapt<ServerResponse>();
            var hasServerStatus = serverStatusesByServerId.TryGetValue(server.Id, out var lastServerStatus);

            if (!hasServerStatus)
            {
                logger.LogDebug("No serverStatus found for server {ServerId}", server.Id);
            }

            return serverResponse with
            {
                LatestStatus = lastServerStatus
            };
        }).ToList();

        var serversWithServerStatus = serverResponses.Count(s => s.LatestStatus != null);

        logger.LogInformation("Retrieved {ServerCount} servers ({WithServerStatusCount} with serverStatus, {WithoutServerStatusCount} without serverStatus). Meta data: {@MetaData}",
            serverResponses.Count, serversWithServerStatus, serverResponses.Count - serversWithServerStatus, servers.MetaData);

        return new PagedList<ServerResponse>(serverResponses, servers.MetaData);
    }

    public async Task<ServerDetailedResponse?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving detailed server information for ID: {ServerId}", id);

        Server server = await serversRepository.GetByIdAsync(id, cancellationToken) ?? throw new ServerNotFoundException(id);

        logger.LogDebug("Server {ServerId} found: {ServerName} ({AppName})", id, server.HostName, server.AppName);

        var serverStatusQueryParameters = new ServerStatusQueryParameters(WithServerId: id, OrderBy: $"{nameof(ServerStatusResponse.RecordedAt)} desc", PageSize: 10);

        var serverStatusesTask = serverStatusesRepository.GetPagedListByQueryAsync(new ServerStatusQuery(serverStatusQueryParameters), cancellationToken);

        await Task.WhenAll(serverStatusesTask);

        var serverStatuses = await serverStatusesTask;

        logger.LogDebug("Retrieved associated data for server {ServerId}: {ServerStatusCount} serverStatuses", id, serverStatuses.Count);

        var serverResponse = server.Adapt<ServerDetailedResponse>();

        logger.LogInformation("Successfully retrieved detailed server {ServerId} with {ServerStatusCount} recent serverStatuses", id, serverStatuses.Count);

        return serverResponse with
        {
            Statuses = serverStatuses.Adapt<IList<ServerStatusResponse>>()
        };
    }

    public async Task<ServerResponse> CreateAsync(ServerCreateRequest serverRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new server with HostName: {HostName} AppName: {AppName}", serverRequest.HostName, serverRequest.AppName);

        var server = serverRequest.Adapt<Server>() with
        {
            Id = ServerIdGenerator.Generate(serverRequest.HostName, serverRequest.AppName)
        };

        await serversRepository.CreateAsync(server, cancellationToken);

        logger.LogInformation("Successfully created server with ID: {ServerId}", server.Id);

        return server.Adapt<ServerResponse>();
    }

    public async Task UpdateAsync(string id, ServerUpdateRequest serverRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating server with ID: {ServerId}", id);

        var server = await serversRepository.GetByIdAsync(id, cancellationToken) ?? throw new ServerNotFoundException(id);

        serverRequest.Adapt(server);

        logger.LogDebug("Applying updates to server {ServerId}: HostName={NewName}, AppName={NewAppName}", id, serverRequest.HostName, serverRequest.AppName);
        await serversRepository.UpdateAsync(server, cancellationToken);

        logger.LogInformation("Successfully updated server with ID: {ServerId}", id);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting server with ID: {ServerId}", id);

        var server = await serversRepository.GetByIdAsync(id, cancellationToken) ?? throw new ServerNotFoundException(id);

        await serversRepository.DeleteAsync(server, cancellationToken);

        logger.LogInformation("Successfully deleted server with ID: {ServerId}", id);
    }
}
