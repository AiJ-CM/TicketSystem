using FluentAssertions;
using TicketSystem.Application.Comments.Commands.CreateComment;

namespace TicketSystem.Application.UnitTests.Comments.Commands.CreateComment;

public class CreateCommentCommandTests
{
    [Fact]
    public void Constructor_Should_Retain_All_Comment_Inputs()
    {
        var command = new CreateCommentCommand(123, "Investigating", true);

        command.TicketId.Should().Be(123);
        command.Content.Should().Be("Investigating");
        command.IsInternal.Should().BeTrue();
    }
}