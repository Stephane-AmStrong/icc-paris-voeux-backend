namespace Domain.Errors;

public sealed class WishNotFoundException(string id) : NotFoundException($"The wish with the identifier '{id}' was not found.");
