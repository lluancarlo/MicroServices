using ChatAPI.Domain.Commands.Responses;
using MediatR;

namespace ChatAPI.Domain.Commands.Requests
{
    public class CreateSessionRequest : IRequest<CreateSessionResponse>
    {
        public string Name { get; set; }
    }
}