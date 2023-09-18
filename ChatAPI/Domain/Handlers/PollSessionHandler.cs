using ChatAPI.Domain.Commands.Requests;
using ChatAPI.Domain.Commands.Responses;
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
            // TODO: Poll chat based on request.id
            _logger.LogInformation($"Chat poll not implemented yet!");
            var result = new PollSessionResponse { Messages = "New messages from Agent X" };

            return await Task.FromResult(result);
        }
    }
}