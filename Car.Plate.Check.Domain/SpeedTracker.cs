using System;

namespace Car.Plate.Check.Domain;

public class SpeedTracker
{
    public Guid Id { get; set; }
    public Car Car { get; set; } = default!;
    public short Velocity { get; set; }
    public DateTime DateTimeUtc { get; set; }
}
