using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ChatCoordinatorService.Services;
using ChatCoordinatorService.DB;
using System.Reflection;
using MassTransit;
using ChatCoordinatorService.Consumers;
using ChatCoordinatorService.Domain;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// Add Services
builder.Services.AddHostedService<CoordinatorService>();

// Configure DB Context
builder.Services.AddDbContext<DatabaseContext>(op =>
    op.UseInMemoryDatabase(builder.Configuration.GetConnectionString("Database")));

// MassTransit
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ"));
        cfg.ConfigureEndpoints(ctx);
        cfg.ReceiveEndpoint("session-queue", x =>
        {
            x.Consumer<SessionConsumer>(ctx);
            x.Bind("ChatAPI");
        });
    });
});
builder.Services.AddScoped<SessionConsumer>();


IHost host = builder.Build();
host.Run();

// MyDatabaseContext db = new MyDatabaseContext(new DbContextOptionsBuilder<MyDatabaseContext>().UseInMemoryDatabase("TEST").Options);
// db.Persons.Add(new Person() { FirstName = "Berend", LastName = "de Jong" });
// db.SaveChanges();
// db.Persons.ForEachAsync((person) => Console.WriteLine($"{person.Id}\t{person.FirstName}\t{person.LastName}"));

// Console.ReadLine();

// Mock agents
