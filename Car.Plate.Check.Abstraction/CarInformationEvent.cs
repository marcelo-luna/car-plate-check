using MediatR;

namespace Car.Plate.Check.Abstraction;

public class CarInformationEvent : IRequest
{
    public string? Plate { get; set; } 
    public short Velocity {get;set;}
    public string? RadarSerialNumber {get;set;}
    public DateTime DateTimeUtc{get;set;}
}
