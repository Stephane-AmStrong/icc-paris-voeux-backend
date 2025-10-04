using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using DataTransfertObjects.Responses;

namespace Application.UseCases.Alerts.GetById;

public class GetAlertByIdQueryHandler(IAlertsService alertsService) : IQueryHandler<GetAlertByIdQuery, AlertDetailedResponse>
{
    public Task<AlertDetailedResponse> HandleAsync(GetAlertByIdQuery query, CancellationToken cancellationToken)
    {
        return alertsService.GetByIdAsync(query.Id, cancellationToken);
    }
}
