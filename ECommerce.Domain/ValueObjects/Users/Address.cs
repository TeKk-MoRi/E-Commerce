using ECommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.ValueObjects.Users
{
    public class Address : ValueObject
    {
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            throw new NotImplementedException();
        }
    }
}
