using ChatAPI.Domain.Commands.Requests;
using ChatAPI.Domain.Commands.Responses;
using ChatAPI.Domain.Publishers;
using MassTransit;
using MediatR;

namespace ChatAPI.Domain.Commands.Handlers
{
    public class CreateSessionHandler : IRequestHandler<CreateSessionRequest, CreateSessionResponse>
    {
        private readonly IBusControl _bus;

        public CreateSessionHandler(IBusControl bus)
        {
            _bus = bus;
        }

        public async Task<CreateSessionResponse> Handle(CreateSessionRequest request, CancellationToken cancellationToken)
        {
            await _bus.Publish(new SessionMessage
            {
                Name = request.Name,
                Active = true,
                CreatedAt = DateTime.Now
            });
            return new CreateSessionResponse { SessionCreated = true };
        }
    }
}