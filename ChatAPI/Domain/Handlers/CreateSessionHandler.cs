using ChatAPI.Domain.Commands.Requests;
using ChatAPI.Domain.Commands.Responses;
using DomainLib;
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
            var message = new SessionMessage
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Active = true,
                CreatedAt = DateTime.Now
            };

            try
            {
                await _bus.Publish(message);
                _logger.LogInformation($"Message '{message.Name}' added to queue!");
                return new CreateSessionResponse { SessionCreated = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending message '{message.Name}' to queue!");
                return new CreateSessionResponse { SessionCreated = false, ErrorMessage = ex.Message };
            }

        }
    }
}