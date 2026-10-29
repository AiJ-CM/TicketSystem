namespace TicketSystem.Application.Common.Interfaces;

public interface ICurrentUser
{
    long UserId { get; }
    string UserName { get; }
}