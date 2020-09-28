using System;
using System.Threading.Tasks;
using Games.Application.Commands.Games;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Games.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GamesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GamesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetGamesCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            if (id == default)
            {
                return BadRequest();
            }

            var entity = await _mediator.Send(new GetGameCommand(id));

            return Ok(entity);
        }


        [HttpGet("{id}/loaned")]
        public async Task<IActionResult> GetLent(Guid id)
        {
            if (id == default)
            {
                return BadRequest();
            }

            var entity = await _mediator.Send(new GetGameLoanedInfoCommand(id));

            return Ok(entity);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateGameCommand command)
        {
            await _mediator.Send(command);

            return Created(nameof(Get), command.Id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] UpdateGameCommand command)
        {
            if (id == default || id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Put(Guid id)
        {
            if (id == default)
            {
                return BadRequest();
            }

            await _mediator.Send(new DeleteGameCommand(id));

            return NoContent();
        }

        [HttpPost("{id}/lent")]
        public async Task<IActionResult> Lent(Guid id, [FromBody] LendGameCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command);

            return NoContent();
        }


        [HttpDelete("{id}/return")]
        public async Task<IActionResult> Return(Guid id)
        {
            await _mediator.Send(new ReturnGameCommand(id));

            return NoContent();
        }
    }
}
