using CoordinatorService.DB;
using DomainLib.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CoordinatorService.Consumers
{
    public class PollSessionConsumer : IConsumer<PollSessionMessage>
    {
        private readonly ILogger<PollSessionMessage> _logger;
        private readonly DatabaseContext _db;
        private readonly IBusControl _bus;

        public PollSessionConsumer(ILogger<PollSessionMessage> logger, DatabaseContext db, IBusControl bus)
        {
            _logger = logger;
            _db = db;
            _bus = bus;
        }

        public Task Consume(ConsumeContext<PollSessionMessage> context)
        {
            var session = _db.Sessions.Where(w => w.Id == context.Message.Id).FirstOrDefault();

            if (session != null)
            {
                session.PollCount += 1;
                _db.SaveChanges();
                _logger.LogInformation($"Session {session.Id} has a new poll. Total polls: {session.PollCount}");
            }
            else
            {
                _logger.LogInformation($"Session {context.Message.Id} not found!");
            }

            return Task.CompletedTask;
        }
    }
}