using System;
using System.Threading.Tasks;
using Games.Application.Commands.Friends;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Games.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FriendsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FriendsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetFriendsCommand command)
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

            var entity = await _mediator.Send(new GetFriendCommand(id));

            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateFriendCommand command)
        {
            await _mediator.Send(command);

            return Created(nameof(Get), command.Id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] UpdateFriendCommand command)
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

            await _mediator.Send(new DeleteFriendCommand(id));

            return NoContent();
        }
    }
}