using Car.Plate.Check.Abstraction;
using MediatR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Car.Plate.Check.Worker;
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IMediator _mediator;


    public Worker(ILogger<Worker> logger, IConnection connection, IMediator mediator)
    {
        _logger = logger;
        _connection = connection;
        _channel = _connection.CreateModel();
        _mediator = mediator;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel.QueueDeclare(queue: "car-queue",
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var carInformation = System.Text.Json.JsonSerializer.Deserialize<CarInformationEvent>(message);
            //Console.WriteLine(" [x] Received {0}", message);

           if (carInformation != null)
                await _mediator.Send(carInformation, stoppingToken);
        };

        // Start consuming messages
        _channel.BasicConsume(queue: "car-queue",
                             autoAck: true,
                             consumer: consumer);


        // Keep the background service running indefinitely until the application is stopped
        await Task.Delay(Timeout.Infinite, stoppingToken);
        // }
    }
}
