using Car.Plate.Check.Abstraction;
using MediatR;

namespace Car.Plate.Check.Command;

public class CarInformationCommandHandler : IRequestHandler<CarInformationEvent>
{

    

    public async Task Handle(CarInformationEvent request, CancellationToken cancellationToken)
    {
        var car = new Car.Plate.Check.Domain.Car
        {
            Id = Guid.NewGuid(),
            Plate = request.Plate,
            CreateDateTimeUtc = request.DateTimeUtc
        };

        var radar = new Car.Plate.Check.Domain.Radar
        {
            Id = Guid.NewGuid(),
            SerialNumber = request.RadarSerialNumber
        };

        var speedTracker = new Car.Plate.Check.Domain.SpeedTracker
        {
            Id = Guid.NewGuid(),
            Car = car,
            DateTimeUtc = request.DateTimeUtc,
            Velocity = request.Velocity
        };


        Console.WriteLine($"Car with plate {car.Plate} was detected by radar with serial number {radar.SerialNumber} at {speedTracker.DateTimeUtc} with velocity {speedTracker.Velocity}");

       await Task.CompletedTask;
    }
}
