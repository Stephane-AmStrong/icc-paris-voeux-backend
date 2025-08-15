namespace Domain.Errors;

public sealed class UserNotFoundException(string id) : NotFoundException($"The user with the identifier '{id}' was not found.");
