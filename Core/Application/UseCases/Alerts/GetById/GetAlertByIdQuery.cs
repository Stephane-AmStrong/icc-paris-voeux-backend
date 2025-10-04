using Application.Abstractions.Handlers;
using DataTransfertObjects.Responses;

namespace Application.UseCases.Alerts.GetById;

public record GetAlertByIdQuery(string Id) : IQuery<AlertDetailedResponse>;
