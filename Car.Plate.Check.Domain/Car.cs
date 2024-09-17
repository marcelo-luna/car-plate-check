using System;

namespace Car.Plate.Check.Domain;

public class Car
{
    public Guid Id { get; set; }
    public string? Plate { get; set; }
    public DateTime CreateDateTimeUtc{get;set;}
}
