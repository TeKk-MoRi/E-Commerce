using ECommerce.Application.Common;
using MediatR;

namespace ECommerce.Application.Usecases.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler(IApplicationUnitOfWork applicationUnitOfWork)
        : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IApplicationUnitOfWork _uow = applicationUnitOfWork;

        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken = default)
        {
            //var user = new User
            //{
            //    FirstName = request.FirstName,
            //    LastName = request.LastName,
            //    Email = request.Email
            //};
            //_uow.Users.Add(user);
            //await _uow.SaveChangesAsync(cancellationToken);
            //return user.Id;

            return Guid.Empty;
        }
    }
}
