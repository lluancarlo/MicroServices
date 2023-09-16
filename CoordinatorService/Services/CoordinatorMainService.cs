using CoordinatorService.DB;
using CoordinatorService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CoordinatorService.Services;

public class CoordinatorMainService : IHostedService
{
    private readonly ILogger _logger;
    private readonly DatabaseContext _db;

    public CoordinatorMainService(ILogger<CoordinatorMainService> logger, DatabaseContext db)
    {
        _logger = logger;
        _db = db;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[CoordinatorMainService] StartAsync has been called.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[CoordinatorMainService] StopAsync has been called.");
        return Task.CompletedTask;
    }
}