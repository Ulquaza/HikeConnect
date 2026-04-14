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

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromQuery] Guid authorId,
            [FromQuery] Guid targetId,
            CancellationToken cancellationToken)
        {
            var report = await _compatibilityReportService.CreateAsync(authorId, targetId, cancellationToken);
            return report is null
                ? BadRequest()
                : Ok(report);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var reports = await _compatibilityReportService.GetAllAsync(cancellationToken);
            return Ok(reports);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var report = await _compatibilityReportService.GetByIdAsync(id, cancellationToken);
            return report is null
                ? NotFound()
                : Ok(report);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetByUsers(
            [FromQuery] Guid authorId,
            [FromQuery] Guid targetId,
            CancellationToken cancellationToken)
        {
            var report = await _compatibilityReportService.GetByUsersIdAsync(authorId, targetId, cancellationToken);
            return report is null
                ? NotFound()
                : Ok(report);
        }

        [HttpGet("author/{authorId:guid}")]
        public async Task<IActionResult> GetByAuthorId(Guid authorId, CancellationToken cancellationToken)
        {
            var reports = await _compatibilityReportService.GetByAuthorIdAsync(authorId, cancellationToken);
            return Ok(reports);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _compatibilityReportService.DeleteAsync(id, cancellationToken);
            return Ok();
        }
    }
}
