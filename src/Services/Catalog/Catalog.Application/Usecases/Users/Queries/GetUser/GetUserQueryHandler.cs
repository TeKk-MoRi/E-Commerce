using ECommerce.BuildingBlocks.Application;
using Catalog.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Usecases.Users.Queries.GetUser
{
    public class GetUserQueryHandler(IApplicationUnitOfWork applicationUnitOfWork)
        : IRequestHandler<GetUserQuery, GetUserDto>
    {
        private readonly IApplicationUnitOfWork _uow = applicationUnitOfWork;

        public async Task<GetUserDto> Handle(GetUserQuery request,
                CancellationToken cancellationToken = default)
            // => await _uow.Users
            //              .Select(x => new GetUserDto(Guid.Empty, x.Gender, x.Email))
            //              .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            => null;
    }
}
