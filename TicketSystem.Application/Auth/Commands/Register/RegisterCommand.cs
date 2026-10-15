using MediatR;

namespace TicketSystem.Application.Auth.Commands.Register;

public record RegisterCommand(
    string Username,
    string Password,
    string Email
) : IRequest<long>;