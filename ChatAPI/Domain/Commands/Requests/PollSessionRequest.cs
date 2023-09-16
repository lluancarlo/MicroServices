using ChatAPI.Domain.Commands.Responses;
using MediatR;

namespace ChatAPI.Domain.Commands.Requests
{
    public class PollSessionRequest : IRequest<PollSessionResponse>
    {
        public Guid Id { get; set; }
    }
}