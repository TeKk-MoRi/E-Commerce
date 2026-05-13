using ECommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Usecases.Users.Queries.GetUser
{
    public record GetUserDto(Guid Id, Gender Gender, string Email);
}
