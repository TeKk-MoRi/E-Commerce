using ECommerce.Domain.Enums;

namespace ECommerce.Application.Usecases.Users.Queries.GetUser
{
    public record GetUserDto(Guid Id, Gender Gender, string Email);
}
