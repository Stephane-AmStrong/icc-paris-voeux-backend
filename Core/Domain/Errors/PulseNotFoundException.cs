namespace Domain.Errors;

public sealed class PulseNotFoundException(string id) : NotFoundException($"The pulse with the identifier '{id}' was not found.");
