using Riok.Mapperly.Abstractions;
using TicketSystem.Application.Auth.Dtos;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Auth.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UserProfileMapper
{
    private static string MapToUserName(User source)
    {
        return !string.IsNullOrEmpty(source.DisplayName)
            ? source.DisplayName
            : source.UserName;
    }

    [MapProperty(
        nameof(User),
        nameof(UserProfileDto.UserName),
        Use = nameof(MapToUserName))]
    public static partial UserProfileDto ToProfileDto(User user);
}