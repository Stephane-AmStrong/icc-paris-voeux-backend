using Domain.Entities;
using Domain.Shared.Common;

namespace Application.DataTransfertObjects;

public record class WishQueryParameters(string? WithEmail = null): QueryParameters<Wish>;
