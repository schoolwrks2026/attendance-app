using System;
using GymTurf.Domain.Common;

namespace GymTurf.Domain.Entities;

public class Facility : BaseEntity
{
    public Guid BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Gym, Turf, Court
    public int Capacity { get; set; }
    public decimal PricePerHour { get; set; } // Kept as metadata for booking description, even though booking is free
    public bool IsActive { get; set; } = true;
}
