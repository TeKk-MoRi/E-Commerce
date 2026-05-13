using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Tests.TestHelpers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.Tests.Product.CreateProduct
{
    public class Create_WhenPriceIsNegative_ShouldThrow
    {

        [Fact]
        public void Create_Should_Throw_DomainException_When_Price_Is_Negative()
        {
            var act = () => new ProductBuilder().WithPrice(-500).Build();

            act.Should().Throw<DomainException>();
        }
    }
}
