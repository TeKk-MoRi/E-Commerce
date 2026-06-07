using ECommerce.Domain.Enums;
using ECommerce.Domain.ValueObjects.Users;

namespace ECommerce.Domain.Entities.Users
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Address Address { get; set; }
        public Gender Gender { get; set; }
    }
}
