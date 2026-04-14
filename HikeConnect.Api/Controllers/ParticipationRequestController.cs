using HikeConnect.Api.Extensions;
using HikeConnect.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HikeConnect.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipationRequestController : ControllerBase
    {
        private readonly IParticipationRequestService _participationRequestService;

        public ParticipationRequestController(IParticipationRequestService participationRequestService)
        {
            _participationRequestService = participationRequestService;
        }

        [HttpPost("{tripId:guid}")]
        public async Task<IActionResult> Create(Guid tripId, CancellationToken ct)
        {
            if (!User.TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var request = await _participationRequestService.CreateAsync(tripId, userId, ct);
            return request is null
                ? BadRequest()
                : Ok(request);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var request = await _participationRequestService.GetByIdAsync(id, ct);
            return request is null
                ? NotFound()
                : Ok(request);
        }

        [HttpGet("trip/{tripId:guid}")]
        public async Task<IActionResult> GetByTripId(Guid tripId, CancellationToken ct)
        {
            var requests = await _participationRequestService.GetByTripIdAsync(tripId, ct);
            return Ok(requests);
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetByUserId(Guid userId, CancellationToken ct)
        {
            var requests = await _participationRequestService.GetByUserIdAsync(userId, ct);
            return Ok(requests);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{requestId:guid}")]
        public async Task<IActionResult> Update(Guid requestId, CancellationToken ct)
        {
            var request = await _participationRequestService.UpdateAsync(requestId, ct);
            return request is null
                ? NotFound()
                : Ok(request);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{requestId:guid}")]
        public async Task<IActionResult> Delete(Guid requestId, CancellationToken ct)
        {
            await _participationRequestService.DeleteAsync(requestId, ct);
            return Ok();
        }

        [HttpPatch("{requestId:guid}/approve")]
        public async Task<IActionResult> Approve(Guid requestId, CancellationToken ct)
        {
            if (!User.TryGetUserId(out var organizerId))
            {
                return Unauthorized();
            }

            var request = await _participationRequestService.ApproveAsync(requestId, organizerId, ct);
            return request is null
                ? BadRequest()
                : Ok(request);
        }

        [HttpPatch("{requestId:guid}/reject")]
        public async Task<IActionResult> Reject(Guid requestId, CancellationToken ct)
        {
            if (!User.TryGetUserId(out var organizerId))
            {
                return Unauthorized();
            }

            var request = await _participationRequestService.RejectAsync(requestId, organizerId, ct);
            return request is null
                ? BadRequest()
                : Ok(request);
        }

        [HttpPatch("{requestId:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid requestId, CancellationToken ct)
        {
            if (!User.TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var request = await _participationRequestService.CancelAsync(requestId, userId, ct);
            return request is null
                ? BadRequest()
                : Ok(request);
        }
    }
}
