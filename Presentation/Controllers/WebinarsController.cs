using Application.Webinars.Commands.CreateWebinar;
using Application.Webinars.Queries.GetWebinarById;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public sealed class WebinarsController : ApiController
    {

        [HttpGet("{webinarId:guid}")]
        [ProducesResponseType(typeof(WebinarResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWebinar(Guid webinarId, CancellationToken cancellationToken)
        {
            var query = new GetWebinarByIdQuery(webinarId);

            var webinar = await Sender.Send(query, cancellationToken);

            return Ok(webinar);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateWebinar([FromBody] CreateWebinarRequest request, CancellationToken cancellationToken)
        {
            var command = request.Adapt<CreateWebinarCommand>();

            var webinarId = await Sender.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetWebinar), new { webinarId }, webinarId);
        }
    }
}
