using ChatAPI.Domain.Commands.Requests;
using ChatAPI.Domain.Commands.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatAPI.Controllers
{
    [ApiController]
    [Route("v1/[Controller]")]
    public class SessionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SessionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("Create")]
        public Task<CreateSessionResponse> Create([FromBody] CreateSessionRequest command)
        {
            return _mediator.Send(command);
        }

        [HttpGet]
        [Route("Poll")]
        public Task<PollSessionResponse> Poll([FromQuery] PollSessionRequest command)
        {
            return _mediator.Send(command);
        }
    }
}