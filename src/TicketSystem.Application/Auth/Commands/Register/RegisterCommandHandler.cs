using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;
using static BCrypt.Net.BCrypt;

namespace TicketSystem.Application.Auth.Commands.Register;

public class RegisterCommandHandler(ITicketingDbContext db, TimeProvider timeProvider) : IRequestHandler<RegisterCommand, long>
{
    public async Task<long> Handle(
        RegisterCommand request, CancellationToken ct)
    {
        if (await db.Users.AnyAsync(u =>string.Equals(u.UserName, request.Username, StringComparison.OrdinalIgnoreCase), ct))
        {
            throw new BusinessException("Username already exists.", ErrorCodeEnum.UserCodeDuplicate);
        }

        if (await db.Users.AnyAsync(u => string.Equals(u.Email, request.Email, StringComparison.OrdinalIgnoreCase), ct))
        {
            throw new BusinessException("Email is already registered.", ErrorCodeEnum.EmailDuplicate);
        }

        var user = new User
        {
            UserName = request.Username,
            PasswordHash = HashPassword(request.Password),
            Email = request.Email,
            IsActive = true,
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
            CreatedBy = request.Username,
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        return user.Id;
    }
}