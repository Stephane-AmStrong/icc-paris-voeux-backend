#nullable enable
using Application.Abstractions.Handlers;
using Domain.Shared.Common;
using DataTransfertObjects.QueryParameters;
using DataTransfertObjects.Responses;

namespace Application.UseCases.Users.GetByQuery;

public record GetUserQuery(UserQueryParameters Parameters) : IQuery<PagedList<UserResponse>>;
