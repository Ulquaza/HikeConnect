using HikeConnect.Core.Entities;
using HikeConnect.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HikeConnect.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private readonly ITripService _tripService;

        public TripController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var trips = await _tripService.GetAllAsync(ct);
            return Ok(trips);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var trip = await _tripService.GetByIdAsync(id, ct);
            return trip is null
                ? NotFound()
                : Ok(trip);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Trip trip, CancellationToken ct)
        {
            var createdTrip = await _tripService.CreateAsync(trip, ct);
            if (!User.TryGetUserId(out var authorId))
            {
                return Unauthorized();
            }

            var createdTrip = await _tripService.CreateAsync(trip, authorId, ct);
            return createdTrip is null
                ? BadRequest()
                : Ok(createdTrip);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Trip trip, CancellationToken ct)
        {
            var updatedTrip = await _tripService.UpdateAsync(trip, ct);
            if (!User.TryGetUserId(out var authorId))
            {
                return Unauthorized();
            }

            var updatedTrip = await _tripService.UpdateAsync(trip, authorId, ct);
            return updatedTrip is null
                ? NotFound()
                : Ok(updatedTrip);
        }

        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await _tripService.DeleteAsync(id, ct);
            if (!User.TryGetUserId(out var authorId))
            {
                return Unauthorized();
            }

            await _tripService.DeleteAsync(id, authorId, ct);
            return Ok();
        }

        [Authorize]
        [HttpPatch("{id:guid}/publish")]
        public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
        {
            var trip = await _tripService.PublishAsync(id, ct);
            if (!User.TryGetUserId(out var authorId))
            {
                return Unauthorized();
            }

            var trip = await _tripService.PublishAsync(id, authorId, ct);
            return trip is null
                ? NotFound()
                : Ok(trip);
        }

        [Authorize]
        [HttpPatch("{id:guid}/unpublish")]
        public async Task<IActionResult> Unpublish(Guid id, CancellationToken ct)
        {
            var trip = await _tripService.UnpublishAsync(id, ct);
            if (!User.TryGetUserId(out var authorId))
            {
                return Unauthorized();
            }

            var trip = await _tripService.UnpublishAsync(id, authorId, ct);
            return trip is null
                ? NotFound()
                : Ok(trip);
        }
    }
}
