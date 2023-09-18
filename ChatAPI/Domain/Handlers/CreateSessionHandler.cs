using ChatAPI.Domain.Commands.Requests;
using ChatAPI.Domain.Commands.Responses;
using DomainLib.Contracts;
using MassTransit;
using MediatR;

namespace ChatAPI.Domain.Commands.Handlers
{
    public class CreateSessionHandler : IRequestHandler<CreateSessionRequest, CreateSessionResponse>
    {
        private readonly IBusControl _bus;
        private readonly ILogger _logger;

        public CreateSessionHandler(ILogger<CreateSessionHandler> logger, IBusControl bus)
        {
            _logger = logger;
            _bus = bus;
        }

        public async Task<CreateSessionResponse> Handle(CreateSessionRequest request, CancellationToken cancellationToken)
        {
            var message = new CreateSessionMessage
            {
                Id = Guid.NewGuid(),
                CustomerName = request.CustomerName,
                CreatedAt = DateTime.Now
            };

            try
            {
                // Refactor to add MassTransit Requests instead
                await _bus.Publish(message);
                _logger.LogInformation($"Message '{message.CustomerName}' added to queue!");
                return new CreateSessionResponse { SessionId = message.Id, SessionCreated = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending message '{message.CustomerName}' to queue!");
                return new CreateSessionResponse { SessionCreated = false, ErrorMessage = ex.Message };
            }

        }
    }
}