using ChatCoordinatorService.DB;
using ChatCoordinatorService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChatCoordinatorService.Services;

public sealed class CoordinatorService : IHostedService
{
    private readonly ILogger _logger;
    private readonly DatabaseContext _db;

    public CoordinatorService(ILogger<CoordinatorService> logger,
                              DatabaseContext db)
    {
        _logger = logger;
        _db = db;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("1. StartAsync has been called.");
        _db.Agents.Add(new Agent { Id = Guid.NewGuid(), Name = "Test" });
        _db.SaveChanges();

        _db.Agents.ForEachAsync((agent) => Console.WriteLine($"{agent.Id}\t{agent.Name}"));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("4. StopAsync has been called.");

        return Task.CompletedTask;
    }
}