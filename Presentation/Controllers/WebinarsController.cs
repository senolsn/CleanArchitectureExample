using Application.Webinars.Commands.CreateWebinar;
using Application.Webinars.Queries.GetWebinarById;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public sealed class WebinarsController : ApiController
    {
        //Senaryo: ASP.NET Core'dan Minimal API'ye Geçiş. Bu gibi durumlarda WebAPI'e bağımlı oluruz. Bunu önlemek için Controller'ları presentation katmanına taşırız.
        //Bir diğer unsur ise WebAPI'da (normalde de olmamalı zaten ama) logic işlemleri kesinlikle olmamalıdır. Yarın öbürgün Web katmanı değiştiğinde bu logic'lerin de taşınması gerekir.
        //Normalde de logic'ler Web katmanında olmamalı fakat projelerde git gide karmaşıklık arttıkça bazı durumlar ortaya çıkabilir. Bu durumlardan dolayı Presentation katmanı oluşturarak ileriye dönük
        //defansif bir yaklaşım benimsenmiş olur. İleride herhangi bir framework değişikliği olsa da (RestAPI => MınımalAPI) projemiz minimum şekilde etkilenecektir. 

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
