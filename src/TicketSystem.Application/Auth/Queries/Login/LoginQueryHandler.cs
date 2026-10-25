using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Common.Exceptions;
using TicketSystem.Application.Common.Interfaces;
using static BCrypt.Net.BCrypt;

namespace TicketSystem.Application.Auth.Queries.Login;

public class LoginQueryHandler(
    ITicketingDbContext db,
    ITokenService tokenService) : IRequestHandler<LoginQuery, LoginResult>
{
    public async Task<LoginResult> Handle(
        LoginQuery request, CancellationToken cancellationToken)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.UserName == request.Username, cancellationToken);

        if (user == null || !user.IsActive || !Verify(request.Password, user.PasswordHash))
        {
            throw new BusinessException("Invalid username or password", ErrorCodeEnum.LoginFailed);
        }

        var token = tokenService.CreateToken(user);

        return new LoginResult(
            user.Id,
            user.UserName,
            token);
    }
}