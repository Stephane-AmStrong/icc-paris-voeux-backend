#nullable enable
using Application.Abstractions.Handlers;
using DataTransfertObjects.QueryParameters;
using DataTransfertObjects.Responses;
using Domain.Shared.Common;

namespace Application.UseCases.Alerts.GetByQuery;

public record GetAlertQuery(AlertQueryParameters Parameters) : IQuery<PagedList<AlertResponse>>;
