#nullable enable
using DataTransfertObjects.Enumerations;

namespace DataTransfertObjects.Requests;

public record ServerStatusCreateRequest
{
    public string? ServerId { get; init; }
    public ServerStatus? Status { get; init; } = ServerStatus.Up;
}