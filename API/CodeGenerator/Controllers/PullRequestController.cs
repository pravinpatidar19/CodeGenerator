using CodeGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PullRequestController : ControllerBase
    {
        private readonly IPullRequestService _pullRequestService;
        public PullRequestController(IPullRequestService pullRequestService)
        {
            _pullRequestService = pullRequestService;
        }


        [HttpGet("review")]
        public async Task<IActionResult> ReviewPullRequest([FromQuery] string owner,
            [FromQuery] string repo, [FromQuery] int prNumber)
        {
            var result = await _pullRequestService.ReviewPullRequest(owner, repo, prNumber);
            return Ok(result);
        }

    }

}
