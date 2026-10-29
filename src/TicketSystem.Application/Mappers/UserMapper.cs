using Riok.Mapperly.Abstractions;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Mappers;

[Mapper]
public partial class UserMapper
{
    public static string GetName(User user)
    {
        return !string.IsNullOrEmpty(user.DisplayName)
            ? user.DisplayName
            : user.UserName;
    }
}