namespace TicketSystem.Application.Auth.Queries.Login;

public record LoginResult(
    long Id,
    string Username,
    string Email,
    string Token);