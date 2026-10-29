using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Auth.Dtos;
using TicketSystem.Application.Auth.Mappers;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;

namespace TicketSystem.Application.Auth.Queries.GetUser;

public class GetUserProfileQueryHandler(ITicketingDbContext context, ICurrentUser currentUser) : IRequestHandler<GetUserProfileQuery, UserProfileDto>
{
    public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken ct)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == currentUser.UserId, ct);

        return user == null
            ? throw new BusinessException(
                $"User with id '{currentUser.UserId}' not found.",
                errorCode: ErrorCodeEnum.UserNotFound
            )
            : UserProfileMapper.ToProfileDto(user);
    }
}