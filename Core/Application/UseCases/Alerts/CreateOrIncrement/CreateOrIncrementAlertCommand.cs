using Application.Abstractions.Handlers;
using DataTransfertObjects.Requests;
using DataTransfertObjects.Responses;

namespace Application.UseCases.Alerts.CreateOrIncrement;

public record CreateOrIncrementAlertCommand(AlertCreateOrIncrementRequest Payload) : ICommand<AlertResponse>;
