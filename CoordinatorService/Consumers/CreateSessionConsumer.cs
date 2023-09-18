using CoordinatorService.DB;
using CoordinatorService.Domain;
using CoordinatorService.Domain.Enums;
using DomainLib.Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoordinatorService.Consumers
{
    public class CreateSessionConsumer : IConsumer<CreateSessionMessage>
    {
        private const float _capacityMultiplier = 1.5f;
        private readonly ILogger<CreateSessionConsumer> _logger;
        private readonly DatabaseContext _db;
        private readonly IBusControl _bus;

        public CreateSessionConsumer(ILogger<CreateSessionConsumer> logger, DatabaseContext db, IBusControl bus)
        {
            _logger = logger;
            _db = db;
            _bus = bus;
        }

        public Task Consume(ConsumeContext<CreateSessionMessage> context)
        {
            // Check if team has capacity enough
            var team = _db.Agents.Where(w => w.Status == StatusEnum.Online).AsNoTracking().ToList();
            var activeChats = team.Sum(s => s.ActiveChats); //
            var teamCapacity = team.Sum(s => s.Capacity); // 16
            var queueLimit = Math.Floor(teamCapacity * _capacityMultiplier);

            // Active a agent from overflow team if so
            if (activeChats == teamCapacity && !team.Exists(e => e.Shift == ShiftEnum.Overflow))
            {
                _db.Agents.Where(w => w.Shift == ShiftEnum.Overflow && w.Status == StatusEnum.Offline)
                    .FirstOrDefault().Status = StatusEnum.Online;

                queueLimit = _db.Agents.AsNoTracking()
                    .Where(w => w.Status == StatusEnum.Online).Sum(s => s.Capacity) * _capacityMultiplier;
            }

            var new_session = new Session
            {
                Id = context.Message.Id,
                CustomerName = context.Message.CustomerName,
                Active = true,
                CreatedAt = context.Message.CreatedAt,
                PollCount = 0
            };

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

                // _bus.Publish(newChat);
                _logger.LogInformation($"Chat {context.Message.Id} assignet to agent {agent.Name} which has seniority of {agent.Seniority}");
            }
            else
            {
                new_session.Active = false;
                // Refuse Chat
                _logger.LogInformation($"Chat {context.Message.Id} refused. Queue limit reached.");
            }

            _db.Sessions.Add(new_session);
            _db.SaveChanges();

            return Task.CompletedTask;
        }
    }
}