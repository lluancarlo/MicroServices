using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ChatCoordinatorService.Services;
using ChatCoordinatorService.DB;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// Add Services
builder.Services.AddHostedService<CoordinatorService>();

// Configure DB Context
builder.Services.AddDbContext<DatabaseContext>(op =>
    op.UseInMemoryDatabase(builder.Configuration.GetConnectionString("Database")));
// builder.Services.AddScoped<IAgentRepository, AgentRepository>();

IHost host = builder.Build();
host.Run();

// MyDatabaseContext db = new MyDatabaseContext(new DbContextOptionsBuilder<MyDatabaseContext>().UseInMemoryDatabase("TEST").Options);
// db.Persons.Add(new Person() { FirstName = "Berend", LastName = "de Jong" });
// db.SaveChanges();
// db.Persons.ForEachAsync((person) => Console.WriteLine($"{person.Id}\t{person.FirstName}\t{person.LastName}"));

// Console.ReadLine();

// Mock agents
