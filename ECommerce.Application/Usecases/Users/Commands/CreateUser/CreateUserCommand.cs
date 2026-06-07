using MediatR;

namespace ECommerce.Application.Usecases.Users.Commands.CreateUser
{
    public record CreateUserCommand : IRequest<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
