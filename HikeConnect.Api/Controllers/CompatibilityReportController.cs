using HikeConnect.Api.Extensions;
using HikeConnect.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HikeConnect.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CompatibilityReportController : ControllerBase
    {
        private readonly ICompatibilityReportService _compatibilityReportService;

        public CompatibilityReportController(ICompatibilityReportService compatibilityReportService)
        {
            _compatibilityReportService = compatibilityReportService;
        }

        [HttpPost("{targetId:guid}")]
        public async Task<IActionResult> Create(Guid targetId, CancellationToken cancellationToken)
        {
            if (!User.TryGetUserId(out var authorId))
            {
                return Unauthorized();
            }

            var report = await _compatibilityReportService.CreateAsync(authorId, targetId, cancellationToken);
            return report is null
                ? BadRequest()
                : Ok(report);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var reports = await _compatibilityReportService.GetAllAsync(cancellationToken);
            return Ok(reports);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var report = await _compatibilityReportService.GetByIdAsync(id, cancellationToken);
            return report is null
                ? NotFound()
                : Ok(report);
        }

        [HttpGet("target/{targetId:guid}")]
        public async Task<IActionResult> GetByTargetId(Guid targetId, CancellationToken cancellationToken)
        {
            if (!User.TryGetUserId(out var authorId))
            {
                return Unauthorized();
            }

            var report = await _compatibilityReportService.GetByUsersIdAsync(authorId, targetId, cancellationToken);
            return report is null
                ? NotFound()
                : Ok(report);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("author/{authorId:guid}")]
        public async Task<IActionResult> GetByAuthorId(Guid authorId, CancellationToken cancellationToken)
        {
            var reports = await _compatibilityReportService.GetByAuthorIdAsync(authorId, cancellationToken);
            return Ok(reports);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _compatibilityReportService.DeleteAsync(id, cancellationToken);
            return Ok();
        }
    }
}
