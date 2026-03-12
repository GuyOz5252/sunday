using DotResults;
using Sunday.Application.Abstract;
using Sunday.Core.Abstract;
using Sunday.Core.Models;

namespace Sunday.Application.Boards.Create;

public class CreateBoardCommandHandler : ICommandHandler<CreateBoardCommand, string>
{
    private readonly IBoardRepository _boardRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBoardCommandHandler(IBoardRepository boardRepository, IUnitOfWork unitOfWork)
    {
        _boardRepository = boardRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> HandleAsync(CreateBoardCommand command,
        CancellationToken cancellationToken = default)
    {
        var board = new Board
        {
            Name = command.Name,
            BoardMembers = command.BoardMemberIds
                .Select(id => new BoardMember
                {
                    UserId = id,
                    Role = BoardMemberRole.Worker
                })
                .ToList()
        };
        await _boardRepository.CreateBoardAsync(board, cancellationToken);
        var result = await _unitOfWork.CommitAsync(cancellationToken);
        return result.IsFailure
            ? result.Error
            : Result.Success(board.Id);
    }
}
