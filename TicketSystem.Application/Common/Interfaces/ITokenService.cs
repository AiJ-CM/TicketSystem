using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Common.Interfaces;

public interface ITokenService
{
    string CreateToken(User user);
}