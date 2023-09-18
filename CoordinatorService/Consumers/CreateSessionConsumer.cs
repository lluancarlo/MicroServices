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

        public async Task Consume(ConsumeContext<CreateSessionMessage> context)
        {
            // Check if team has capacity enough
            var team = await _db.Agents.Where(w => w.Status == StatusEnum.Online).ToListAsync();
            var activeChats = team.Sum(s => s.ActiveChats); //
            var teamCapacity = team.Sum(s => s.Capacity); // 16
            var queueLimit = Math.Floor(teamCapacity * _capacityMultiplier);

            // Active a agent from overflow team if so
            // TODO: Add checks to check if it is in office hours too
            if (activeChats == teamCapacity)
            {
                var overflow_agents = await _db.Agents
                    .Where(w => w.Shift == ShiftEnum.Overflow && w.Status == StatusEnum.Offline).ToListAsync();

                if (overflow_agents.Any())
                {
                    var overflow_agent = overflow_agents.FirstOrDefault();
                    overflow_agent.Status = StatusEnum.Online;

                    team.Add(overflow_agent);

                    teamCapacity += overflow_agent.Capacity;
                    queueLimit = Math.Floor(teamCapacity * _capacityMultiplier);
                }
            }

            var new_session = new Session
            {
                Id = context.Message.Id,
                CustomerName = context.Message.CustomerName,
                Active = true,
                CreatedAt = context.Message.CreatedAt,
                PollCount = 0
            };

            // Assign the chat to the next available agent
            if (activeChats < teamCapacity)
            {
                var agent = team.Where(w => w.ActiveChats < w.Capacity).OrderBy(o => o.Seniority).FirstOrDefault();
                agent.ActiveChats += 1;

                var newChat = new ChatMessage
                {
                    SessionId = context.Message.Id,
                    CustomerName = context.Message.CustomerName,
                    AgentId = agent.Id,
                    AgentName = agent.Name,
                    Active = true
                };

                // Publish chat in agent queue
                // _bus.Publish(newChat);
            }
            else
            {
                var sessions = await _db.Sessions.Where(w => w.Active).ToListAsync();
                var chats = activeChats + sessions.Count;
                // Put chat on wait queue if all agents are busy
                if (chats < queueLimit)
                {
                    var newChat = new ChatMessage
                    {
                        SessionId = context.Message.Id,
                        CustomerName = context.Message.CustomerName,
                        AgentId = Guid.Empty,
                        AgentName = String.Empty,
                        Active = false
                    };

                    // Publish chat in agent queue
                    // _bus.Publish(newChat);
                    _logger.LogInformation($"Chat {context.Message.Id} assignet to the wait queue.");
                }
                // Refuse Chat
                else
                {
                    new_session.Active = false;
                    _logger.LogInformation($"Chat {context.Message.Id} refused. Queue limit reached.");
                }
            }

            // Save changes in DB
            await _db.Sessions.AddAsync(new_session);
            await _db.SaveChangesAsync();
        }
    }
}