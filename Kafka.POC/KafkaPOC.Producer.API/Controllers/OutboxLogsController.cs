using KafkaPOC.Producer.Application.Queries.OutboxLogs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KafkaPOC.Producer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OutboxLogsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OutboxLogsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("{eventId:guid}")]
        public async Task<IActionResult> GetByEventId(Guid eventId)
        {
            var result = await _mediator.Send(new GetByEventIdQuery(eventId));
            return Ok(result);
        }
    }
}