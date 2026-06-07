using MediatR;

namespace ECommerce.Application.Usecases.Users.Queries.GetUser
{
    public record GetUserQuery(Guid Id) : IRequest<GetUserDto>;
}
