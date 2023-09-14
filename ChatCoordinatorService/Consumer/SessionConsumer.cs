using System.Text.Json;
using ChatCoordinatorService.Domain;
using MassTransit;

namespace ChatCoordinatorService.Consumers
{
    public class SessionConsumer : IConsumer<SessionMessage>
    {
        private readonly ILogger _logger;

        public SessionConsumer(ILogger<SessionConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SessionMessage> context)
        {
            var serializedMessage = JsonSerializer.Serialize(context.Message, new JsonSerializerOptions { });
            _logger.LogInformation($"Message received: {serializedMessage}");
        }
    }
}