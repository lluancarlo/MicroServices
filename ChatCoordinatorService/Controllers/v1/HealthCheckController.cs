using ChatAPI.Domain.Commands.Requests;
using ChatAPI.Domain.Commands.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatCoordinatorService.Controllers
{
    [ApiController]
    [Route("v1/[Controller]")]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return $"{DateTime.Now:O}";
        }
    }
}