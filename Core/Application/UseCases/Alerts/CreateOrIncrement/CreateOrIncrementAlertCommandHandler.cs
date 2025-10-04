using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using DataTransfertObjects.Responses;

namespace Application.UseCases.Alerts.CreateOrIncrement;

public class CreateOrIncrementAlertCommandHandler(IAlertsService alertsService)
    : ICommandHandler<CreateOrIncrementAlertCommand, AlertResponse>
{
    public Task<AlertResponse> HandleAsync(CreateOrIncrementAlertCommand command, CancellationToken cancellationToken)
    {
        return alertsService.CreateOrIncrementAsync(command.Payload, cancellationToken);
    }
}
