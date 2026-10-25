using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using TicketSystem.Domain.Entities;
using static BCrypt.Net.BCrypt;

namespace TicketSystem.Application.Auth.Commands.Register;

public class RegisterCommandHandler(
    ITicketingDbContext db) : IRequestHandler<RegisterCommand, long>
{
    public async Task<long> Handle(
        RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await db.Users.AnyAsync(u => u.UserName == request.Username, cancellationToken))
        {
            throw new BusinessException("Username already exists.", ErrorCodeEnum.UserCodeDuplicate);
        }

        if (await db.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
        {
            throw new BusinessException("Email is already registered.", ErrorCodeEnum.EmailDuplicate);
        }

        var user = new User
        {
            UserName = request.Username,
            PasswordHash = HashPassword(request.Password),
            Email = request.Email,
            IsActive = true
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}