using MediatR;
using Microsoft.EntityFrameworkCore;
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
            .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

        if (user == null
            || !user.IsActive
            || !Verify(request.Password, user.PasswordHash))
        {
            throw new InvalidOperationException("用户名或密码错误");
        }

        var token = tokenService.CreateToken(user);

        return new LoginResult(
            user.Id,
            user.Username,
            user.Email,
            token);
    }
}