using Car.Plate.Check.Command;
using RabbitMQ.Client;

namespace Car.Plate.Check.Worker;
public static class ServiceCollectionExtesion
{
    public static void AddWorker(this IServiceCollection services)
    {
        services.AddHostedService<Worker>();
    }

    public static void AddRabbitMQ(this IServiceCollection services)
    {
        services.AddSingleton<IConnection>(sp =>
        {
            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq",
                UserName = "guest",
                Password = "guest",
                Port = 5672
            };

            IConnection connection = null;
            int maxAttempts = 10;
            int currentAttempt = 0;

            while (currentAttempt < maxAttempts)
            {
                try
                {
                    connection = factory.CreateConnection();
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to connect to RabbitMQ. Attempt {currentAttempt + 1}/{maxAttempts}. Error: {ex.Message}");
                    currentAttempt++;
                    Thread.Sleep(5000); // Wait for 5 second before retrying
                }
            }

            if (connection == null)
            {
                throw new Exception($"Failed to connect to RabbitMQ after {maxAttempts} attempts.");
            }

            return connection;
        });
    }

    public static void AddMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssemblyContaining(typeof(CarInformationCommandHandler));
        });
    }
}
