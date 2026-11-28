using Riok.Mapperly.Abstractions;
using TicketSystem.Application.Auth.Dtos;
using TicketSystem.Application.Mappers;

namespace TicketSystem.Application.Auth.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UserProfileMapper
{
    [MapProperty(
        nameof(User),
        nameof(UserProfileDto.UserName),
        Use = nameof(@UserMapper.GetName))]
    public static partial UserProfileDto ToProfileDto(User user);
}