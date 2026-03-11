using Sunday.Application.Abstract;

namespace Sunday.Application.Boards.Create;

public class CreateBoardCommand : ICommand<string>
{
    public string Name { get; init; }
    public List<string> BoardMemberIds { get; init; }
}
