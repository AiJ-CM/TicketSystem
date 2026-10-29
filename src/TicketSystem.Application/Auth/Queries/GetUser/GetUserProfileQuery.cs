using MediatR;
using TicketSystem.Application.Auth.Dtos;

namespace TicketSystem.Application.Auth.Queries.GetUser;

public record GetUserProfileQuery : IRequest<UserProfileDto>;