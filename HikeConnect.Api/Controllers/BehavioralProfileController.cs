using HikeConnect.Core.Dtos;
using HikeConnect.Core.Entities;
using HikeConnect.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HikeConnect.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BehavioralProfileController : ControllerBase
    {
        private readonly IBehavioralProfileService _behavioralProfileService;

        public BehavioralProfileController(IBehavioralProfileService behavioralProfileService)
        {
            _behavioralProfileService = behavioralProfileService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] BehavioralSurveySubmissionRequest request,
            CancellationToken cancellationToken)
        {
            var profile = await _behavioralProfileService.CreateAsync(request, cancellationToken);
            if (!User.TryGetUserId(out var authorId))
            {
                return Unauthorized();
            }

            var profile = await _behavioralProfileService.CreateAsync(request, authorId, cancellationToken);
            return profile is null
                ? BadRequest()
                : Ok(profile);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var profiles = await _behavioralProfileService.GetAllAsync(cancellationToken);
            return Ok(profiles);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var profile = await _behavioralProfileService.GetByIdAsync(id, cancellationToken);
            return profile is null
                ? NotFound()
                : Ok(profile);
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetByUserId(Guid userId, CancellationToken cancellationToken)
        {
            var profile = await _behavioralProfileService.GetByUserIdAsync(userId, cancellationToken);
            return profile is null
                ? NotFound()
                : Ok(profile);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] BehavioralProfile profile, CancellationToken cancellationToken)
        {
            var updatedProfile = await _behavioralProfileService.UpdateAsync(profile, cancellationToken);
            if (!User.TryGetUserId(out var authorId))
            {
                return Unauthorized();
            }

            var updatedProfile = await _behavioralProfileService.UpdateAsync(profile, authorId, cancellationToken);
            return updatedProfile is null
                ? NotFound()
                : Ok(updatedProfile);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _behavioralProfileService.DeleteAsync(id, cancellationToken);
            return Ok();
        }
    }
}
