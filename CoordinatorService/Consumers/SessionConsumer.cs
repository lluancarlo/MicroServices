using CoordinatorService.DB;
using CoordinatorService.Domain.Enums;
using DomainLib.Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoordinatorService.Consumers
{
    public class SessionConsumer : IConsumer<SessionMessage>
    {
        private const float _capacityMultiplier = 1.5f;
        private readonly ILogger<SessionConsumer> _logger;
        private readonly DatabaseContext _db;
        private readonly IBusControl _bus;

        public SessionConsumer(ILogger<SessionConsumer> logger, DatabaseContext db, IBusControl bus)
        {
            _logger = logger;
            _db = db;
            _bus = bus;
        }

        public Task Consume(ConsumeContext<SessionMessage> context)
        {
            // Check if team has capacity enough
            var team = _db.Agents.Where(w => w.Status == StatusEnum.Online).AsNoTracking().ToList();
            var activeChats = team.Sum(s => s.ActiveChats); // 16
            var teamCapacity = team.Sum(s => s.Capacity); // 16
            var queueLimit = teamCapacity * _capacityMultiplier; // 24

            // Active a agent from overflow team if so
            if (activeChats == teamCapacity && !team.Exists(e => e.Shift == ShiftEnum.Overflow))
            {
                _db.Agents.Where(w => w.Shift == ShiftEnum.Overflow && w.Status == StatusEnum.Offline)
                    .FirstOrDefault().Status = StatusEnum.Online;

                queueLimit = _db.Agents.AsNoTracking()
                    .Where(w => w.Status == StatusEnum.Online).Sum(s => s.Capacity) * _capacityMultiplier;
            }

            if (activeChats < queueLimit)
            {
                // Assign the chat to the next available agent
                var agent = _db.Agents.Where(w => w.Status == StatusEnum.Online)
                    .OrderBy(o => o.Seniority).FirstOrDefault();

                var newChat = new ChatMessage
                {
                    SessionId = context.Message.Id,
                    CustomerName = context.Message.CustomerName,
                    AgentId = agent.Id,
                    AgentName = agent.Name,
                    Active = true
                };

                _logger.LogInformation($"Chat {context.Message.Id} assignet to agent {agent.Name} which has seniority of {agent.Seniority}");
                _bus.Publish(newChat);
            }
            else
            {
                // Refuse Chat
                _logger.LogInformation($"Chat {context.Message.Id} refused. Queue limit reached.");
            }

            return Task.CompletedTask;
        }
    }
}