using Bogus;
using CoordinatorService.DB;
using CoordinatorService.Domain;
using CoordinatorService.Domain.Enums;
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

        LoadAgents();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[CoordinatorMainService] StopAsync has been called.");
        return Task.CompletedTask;
    }

    private void LoadAgents()
    {
        // Bake Agents
        List<Agent> bakedAgents = new List<Agent>();

        // Team A: 1x team lead, 2x mid-level, 1x junior
        var list = new List<SeniorityEnum>() {
            SeniorityEnum.Lead,
            SeniorityEnum.Mid,
            SeniorityEnum.Mid,
            SeniorityEnum.Junior
        };
        list.ForEach(seniority =>
        {
            bakedAgents.Add(new Faker<Agent>()
                .RuleFor(r => r.Id, f => Guid.NewGuid())
                .RuleFor(r => r.Name, f => f.Name.FullName())
                .RuleFor(r => r.Seniority, f => seniority)
                .RuleFor(r => r.Shift, f => ShiftEnum.Morning)
                .RuleFor(r => r.Status, f => StatusEnum.Online)
            );
        });

        // Team B: 1x senior, 1x mid-level, 2x junior
        list = new List<SeniorityEnum>() {
            SeniorityEnum.Senior,
            SeniorityEnum.Mid,
            SeniorityEnum.Junior,
            SeniorityEnum.Junior
        };
        list.ForEach(seniority =>
        {
            bakedAgents.Add(new Faker<Agent>()
                .RuleFor(r => r.Id, f => Guid.NewGuid())
                .RuleFor(r => r.Name, f => f.Name.FullName())
                .RuleFor(r => r.Seniority, f => seniority)
                .RuleFor(r => r.Shift, f => ShiftEnum.Afternoon)
                .RuleFor(r => r.Status, f => StatusEnum.Offline)
            );
        });

        // Team C: 2x mid-level (night shift team)
        for (int i = 0; i < 2; i++)
        {
            bakedAgents.Add(new Faker<Agent>()
                .RuleFor(r => r.Id, f => Guid.NewGuid())
                .RuleFor(r => r.Name, f => f.Name.FullName())
                .RuleFor(r => r.Seniority, f => SeniorityEnum.Mid)
                .RuleFor(r => r.Shift, f => ShiftEnum.Overnight)
                .RuleFor(r => r.Status, f => StatusEnum.Offline)
            );
        }

        // Team Overflow: x6 considered Junior.
        for (int i = 0; i < 6; i++)
        {
            bakedAgents.Add(new Faker<Agent>()
                .RuleFor(r => r.Id, f => Guid.NewGuid())
                .RuleFor(r => r.Name, f => f.Name.FullName())
                .RuleFor(r => r.Seniority, f => SeniorityEnum.Junior)
                .RuleFor(r => r.Shift, f => ShiftEnum.Overflow)
                .RuleFor(r => r.Status, f => StatusEnum.Offline)
            );
        }

        _logger.LogInformation($"[CoordinatorMainService] Generated {bakedAgents.Count()} agents.");
        _db.Agents.AddRange(bakedAgents);
        _db.SaveChanges();
    }
}