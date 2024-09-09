namespace Car.Plate.Chek.Device.Worker;
using RabbitMQ.Client;
using Car.Plate.Chek.Device.Worker.Models;
using System.Text;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IModel channel = null!;

        bool isConnected = false;
        while (!isConnected)
        {
            try
            {
                isConnected = TryToConnect(out channel);
            }
            catch (System.Exception)
            {
                _logger.LogCritical("Connect filed, trying again after 3 seconds");
                await Task.Delay(3000);
            }
        }

        channel.QueueDeclare(queue: "car-queue",
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

        while (!stoppingToken.IsCancellationRequested)
        {
            //if (_logger.IsEnabled(LogLevel.Information))
            //{
            var carInformation = GenerateCarInformation();
            //_logger.LogInformation("New Car Detected: {time}", DateTimeOffset.Now);
            _logger.LogInformation(System.Text.Json.JsonSerializer.Serialize(carInformation));
            //}

            var body = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(carInformation));

            channel.BasicPublish(exchange: string.Empty,
                                routingKey: "car-queue",
                                basicProperties: null,
                                body: body);

            await Task.Delay(1000, stoppingToken);
        }
    }

    private bool TryToConnect(out IModel channel)
    {
        var factory = new ConnectionFactory { HostName = "rabbitmq", UserName = "guest", Password = "guest", Port = 5672 };
        var connection = factory.CreateConnection();
        channel = connection.CreateModel();
        _logger.LogInformation("### RABBIT CONNECTED ###");
        return true;
    }

    private CarInformation GenerateCarInformation()
    {
        return new CarInformation
        {
            Plate = GenerateRandomPlate(),
            Velocity = GenerateRandomVelocity(),
            DateTimeUtc = DateTime.UtcNow
        };
    }

    private string GenerateRandomPlate()
    {
        // Characters that can be used in the random string
        string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        Random random = new Random();

        // Generate two random letters for the beginning
        char firstLetter = letters[random.Next(letters.Length)];
        char secondLetter = letters[random.Next(letters.Length)];

        // Generate three random digits
        int randomNumber = random.Next(100, 1000);

        // Generate two random letters for the end
        char thirdLetter = letters[random.Next(letters.Length)];
        char fourthLetter = letters[random.Next(letters.Length)];

        // Combine the parts into the desired pattern
        return $"{firstLetter}{secondLetter}-{randomNumber}-{thirdLetter}{fourthLetter}";
    }

    private short GenerateRandomVelocity()
    {
        Random random = new Random();
        return (short)random.Next(80, 150);
    }
}
