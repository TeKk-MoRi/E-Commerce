using Catalog.Domain.Enums;

namespace Catalog.Application.Usecases.Users.Queries.GetUser
{
    public record GetUserDto(Guid Id, Gender Gender, string Email);
}
