using MediatR;

namespace TicketSystem.Application.Tickets.Commands.SubmitResolution;

public record SubmitResolutionCommand(
    long TicketId,
    long OperatorId,
    string? ResolutionNote) : IRequest;