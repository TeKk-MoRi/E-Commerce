using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.Common.Events
{
    public abstract class DomainEventBase : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
