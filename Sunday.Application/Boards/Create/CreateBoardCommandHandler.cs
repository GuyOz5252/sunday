using DotResults;
using Sunday.Application.Abstract;
using Sunday.Core.Abstract;

namespace Sunday.Application.Boards.Create;

public class CreateBoardCommandHandler : ICommandHandler<CreateBoardCommand, string>
{
    // private readonly IBoardRepository _boardRepository;
    // private readonly IUnitOfWork _unitOfWork;
    //
    // public CreateBoardCommandHandler(IBoardRepository boardRepository, IUnitOfWork unitOfWork)
    // {
    //     _boardRepository = boardRepository;
    //     _unitOfWork = unitOfWork;
    // }

    public Task<Result<string>> HandleAsync(CreateBoardCommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
