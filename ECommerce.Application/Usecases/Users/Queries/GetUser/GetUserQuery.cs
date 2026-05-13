using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Usecases.Users.Queries.GetUser
{
    public record GetUserQuery(Guid Id) : IRequest<GetUserDto>;
}
