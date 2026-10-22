using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Auth.Dtos;
using TicketSystem.Application.Auth.Mappers;
using TicketSystem.Application.Common.Interfaces;

namespace TicketSystem.Application.Auth.Queries.GetUser;

public class GetUserProfileQueryHandler(ITicketingDbContext context) : IRequestHandler<GetUserProfileQuery, UserProfileDto>
{
    public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken) 
            ?? throw new KeyNotFoundException($"User with id '{request.UserId}' not found.");

        return UserProfileMapper.ToProfileDto(user);
    }
}