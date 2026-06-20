namespace Catalog.Domain.Common;

public abstract class AuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }

    public DateTime? LastModifiedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }
}