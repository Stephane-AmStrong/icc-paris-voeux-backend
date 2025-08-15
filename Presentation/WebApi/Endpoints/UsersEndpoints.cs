using System.Text.Json;
using Application.Abstractions.Handlers;
using Application.UseCases.Users.Create;
using Application.UseCases.Users.Delete;
using Application.UseCases.Users.GetById;
using Application.UseCases.Users.GetByQuery;
using Application.UseCases.Users.Update;
using Domain.Shared.Common;
using DataTransfertObjects.QueryParameters;
using DataTransfertObjects.Requests;
using DataTransfertObjects.Responses;
using WebApi.Models;

namespace WebApi.Endpoints;
public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users");

        group.MapGet("/", GetByQueryParameters)
            .Produces<IList<UserResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", GetUserById)
            .Produces<UserDetailedResponse>(StatusCodes.Status200OK)
            .WithName(nameof(GetUserById));

        group.MapPost("/", CreateUser)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<UserDetailedResponse>(StatusCodes.Status201Created);

        group.MapDelete("/{id}", DeleteUser)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id}", UpdateUser)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
    }

    // GET /api/users
    private static  async Task<IResult> GetByQueryParameters(IQueryHandler<GetUserQuery, PagedList<UserResponse>> handler, [AsParameters] UserQueryParameters queryParameters, HttpResponse response, CancellationToken cancellationToken)
    {
        var usersResponse = await handler.HandleAsync(new GetUserQuery(queryParameters), cancellationToken);

        response.Headers.Append("X-Pagination", JsonSerializer.Serialize(usersResponse.MetaData));

        return Results.Ok(usersResponse);
    }

    // GET /api/users/{id}
    private static async Task<IResult> GetUserById(IQueryHandler<GetUserByIdQuery, UserDetailedResponse?> handler, string id, CancellationToken cancellationToken)
    {
        var userResponse = await handler.HandleAsync(new GetUserByIdQuery(id), cancellationToken);
        return Results.Ok(userResponse);
    }

    // POST /api/users
    private static async Task<IResult> CreateUser(ICommandHandler<CreateUserCommand, UserResponse> handler, UserCreateRequest userRequest, CancellationToken cancellationToken)
    {
        var userResponse = await handler.HandleAsync(new CreateUserCommand(userRequest), cancellationToken);
        return Results.CreatedAtRoute(nameof(GetUserById), new { id = userResponse.Id }, userResponse);
    }

    // DELETE /api/users/{id}
    private static async Task<IResult> DeleteUser(ICommandHandler<DeleteUserCommand> handler, string id, CancellationToken cancellationToken)
    {
        await handler.HandleAsync(new DeleteUserCommand(id), cancellationToken);
        return Results.NoContent();
    }

    // PUT /api/users/{id}
    private static async Task<IResult> UpdateUser(ICommandHandler<UpdateUserCommand> handler, string id, UserUpdateRequest userRequest, CancellationToken cancellationToken)
    {
        await handler.HandleAsync(new UpdateUserCommand(id, userRequest), cancellationToken);
        return Results.NoContent();
    }
}
