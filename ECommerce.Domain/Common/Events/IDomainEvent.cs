using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.Common.Events
{
    public interface IDomainEvent : INotification { }
}
