using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Sunday.Api.Endpoints.Abstract;
using Sunday.Application.Abstract;
using Sunday.Application.Boards.Create;

namespace Sunday.Api.Endpoints.Boards;

public class CreateBoardEndpoint : IEndpoint
{
    private sealed record CreateBoardRequest(string Name, List<string> BoardMemberIds);
    
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/boards", async (
            CreateBoardRequest request,
            ICommandHandler<CreateBoardCommand, string> commandHandler,
            CancellationToken ct) =>
        {
            var result = await commandHandler.HandleAsync(new CreateBoardCommand
            {
                Name = request.Name,
                BoardMemberIds = request.BoardMemberIds
            }, ct);
            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }

            return Results.InternalServerError();
        }).RequireAuthorization();
    }
}
