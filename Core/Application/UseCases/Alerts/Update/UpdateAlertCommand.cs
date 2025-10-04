using Application.Abstractions.Handlers;
using DataTransfertObjects.Requests;

namespace Application.UseCases.Alerts.Update;

public record UpdateAlertCommand(string Id, AlertUpdateRequest Payload) : ICommand;
