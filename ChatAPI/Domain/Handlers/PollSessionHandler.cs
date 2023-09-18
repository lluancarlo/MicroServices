using ChatAPI.Domain.Commands.Requests;
using ChatAPI.Domain.Commands.Responses;
using DomainLib.Contracts;
using MassTransit;
using MediatR;

namespace ChatAPI.Domain.Commands.Handlers
{
    public class PollSessionHandler : IRequestHandler<PollSessionRequest, PollSessionResponse>
    {
        private readonly IBusControl _bus;
        private readonly ILogger _logger;

        public PollSessionHandler(ILogger<PollSessionHandler> logger, IBusControl bus)
        {
            _logger = logger;
            _bus = bus;
        }

        public async Task<PollSessionResponse> Handle(PollSessionRequest request, CancellationToken cancellationToken)
        {
            var message = new PollSessionMessage
            {
                Id = request.Id
            };

            try
            {
                // Refactor to add MassTransit Requests instead
                await _bus.Publish(message);
                _logger.LogInformation($"Polling chat '{message.Id}'!");
                return new PollSessionResponse { PollSuccess = true, ErrorMessage = String.Empty };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error polling chat '{message.Id}'! {ex.Message}");
                return new PollSessionResponse { PollSuccess = false, ErrorMessage = $"Error polling chat '{message.Id}'!" };
            }
        }
    }
}