using System.Text.Json;
using DomainLib;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CoordinatorService.Consumers
{
    public class SessionConsumer : IConsumer<SessionMessage>
    {
        private readonly ILogger<SessionConsumer> _logger;

        public SessionConsumer(ILogger<SessionConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<SessionMessage> context)
        {
            var serializedMessage = JsonSerializer.Serialize(context.Message, new JsonSerializerOptions { });
            _logger.LogInformation($"Message received: {serializedMessage}");
            return Task.CompletedTask;
        }
    }
}