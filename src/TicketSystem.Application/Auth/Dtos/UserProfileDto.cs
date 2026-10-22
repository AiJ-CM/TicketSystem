namespace TicketSystem.Application.Auth.Dtos;
public class UserProfileDto
{
    public long Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}