using MediatR;

namespace TicketSystem.Application.Auth.Queries.Login;

public record LoginQuery(
    string Username,
    string Password) : IRequest<LoginResult>;