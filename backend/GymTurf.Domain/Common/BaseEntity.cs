using System;

namespace GymTurf.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001"); // Static/default tenant ID
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = "System";
    public string UpdatedBy { get; set; } = "System";
    public DateTime? DeletedAt { get; set; }
    public int Version { get; set; } = 1;
}
