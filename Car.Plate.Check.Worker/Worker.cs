namespace Car.Plate.Check.Worker;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
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

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" [x] Received {0}", message);
        };

        // Start consuming messages
        channel.BasicConsume(queue: "car-queue",
                             autoAck: true,
                             consumer: consumer);


        // Keep the background service running indefinitely until the application is stopped
        await Task.Delay(Timeout.Infinite, stoppingToken);
        // }
    }

    private bool TryToConnect(out IModel channel)
    {
        var factory = new ConnectionFactory { HostName = "rabbitmq", UserName = "guest", Password = "guest", Port = 5672 };
        var connection = factory.CreateConnection();
        channel = connection.CreateModel();
        _logger.LogInformation("### RABBIT CONNECTED ###");
        return true;
    }
}
