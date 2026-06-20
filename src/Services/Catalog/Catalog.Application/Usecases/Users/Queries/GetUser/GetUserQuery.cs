using MediatR;

namespace Catalog.Application.Usecases.Users.Queries.GetUser
{
    public record GetUserQuery(Guid Id) : IRequest<GetUserDto>;
}
