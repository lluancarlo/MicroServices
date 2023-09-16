using Bogus;
using CoordinatorService.DB;
using CoordinatorService.Domain;
using CoordinatorService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CoordinatorService.Services;

public class ShiftService : IHostedService
{
    private readonly ILogger _logger;
    private readonly DatabaseContext _db;

    public ShiftService(ILogger<ShiftService> logger, DatabaseContext db)
    {
        _logger = logger;
        _db = db;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[ShiftService] StartAsync has been called.");

        // Bake _db.Agents
        // Team A: 1x team lead, 2x mid-level, 1x junior
        var list = new List<SeniorityEnum>() {
            SeniorityEnum.TeamLead,
            SeniorityEnum.Mid,
            SeniorityEnum.Mid,
            SeniorityEnum.Junior
        };
        list.ForEach(seniority =>
        {
            _db.Agents.Add(new Faker<Agent>()
                .RuleFor(r => r.Id, f => Guid.NewGuid())
                .RuleFor(r => r.Name, f => f.Name.FullName())
                .RuleFor(r => r.Seniority, f => seniority)
                .RuleFor(r => r.Shift, f => ShiftEnum.Morning)
                .RuleFor(r => r.Status, f => StatusEnum.Offline)
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
            _db.Agents.Add(new Faker<Agent>()
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
            _db.Agents.Add(new Faker<Agent>()
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
            _db.Agents.Add(new Faker<Agent>()
                .RuleFor(r => r.Id, f => Guid.NewGuid())
                .RuleFor(r => r.Name, f => f.Name.FullName())
                .RuleFor(r => r.Seniority, f => SeniorityEnum.Junior)
                .RuleFor(r => r.Shift, f => ShiftEnum.Overflow)
                .RuleFor(r => r.Status, f => StatusEnum.Offline)
            );
        }

        // Simulate work shift change
        new Timer((e) =>
        {
            var current = _db.Agents.Where(w => w.Status == StatusEnum.Online).FirstOrDefault();
            if (current == null)
            {
                _db.Agents.Where(w => w.Shift == ShiftEnum.Morning)
                    .ForEachAsync(a => a.Status = StatusEnum.Online);
                _logger.LogInformation($"[ShiftService] Shift started on '{ShiftEnum.Morning}'.");
            }
            else
            {
                _db.Agents.Where(w => w.Shift == current.Shift)
                    .ForEachAsync(a => a.Status = StatusEnum.Offline);

                var next_shift = current.Shift switch
                {
                    ShiftEnum.Morning => ShiftEnum.Afternoon,
                    ShiftEnum.Afternoon => ShiftEnum.Overnight,
                    _ => ShiftEnum.Morning
                };

                _db.Agents.Where(w => w.Shift == next_shift)
                    .ForEachAsync(a => a.Status = StatusEnum.Online);
                _logger.LogInformation($"[ShiftService] Shift changed from '{current.Shift}' to '{next_shift}'.");
            }

            _db.SaveChanges();
        }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[ShiftService] StopAsync has been called.");

        return Task.CompletedTask;
    }
}