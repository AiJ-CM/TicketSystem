namespace TicketSystem.Application.Comments.Dtos;

public class CommentDto
{
    public long Id{ get; set; }
    public long CommenterId{ get; set; }
    public required string CommenterName{ get; set; }
    public required string Content { get; set; }
    public bool IsInternal{ get; set; }
    public DateTime CreatedAt{ get; set; }
    public required string CreatedBy { get; set; }
}
