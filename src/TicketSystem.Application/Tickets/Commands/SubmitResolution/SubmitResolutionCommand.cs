using MediatR;

namespace TicketSystem.Application.Tickets.Commands.SubmitResolution;

public record SubmitResolutionCommand(
    long TicketId,
    string? ResolutionNote) : IRequest;