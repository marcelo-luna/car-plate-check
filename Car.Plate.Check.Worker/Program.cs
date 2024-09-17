using Car.Plate.Check.Worker;

var builder = Host.CreateApplicationBuilder(args);
ServiceCollectionExtesion.AddWorker(builder.Services);
ServiceCollectionExtesion.AddRabbitMQ(builder.Services);
ServiceCollectionExtesion.AddMediatR(builder.Services);

var host = builder.Build();
host.Run();
