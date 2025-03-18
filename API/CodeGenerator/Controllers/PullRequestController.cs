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
            var prResult = await _pullRequestService.ReviewPullRequest(owner, repo, prNumber);

            if (string.IsNullOrEmpty(prResult))
                return Ok("No code changes found in the pull request.");

            var result = await _pullRequestService.ReviewPullRequestCode(prResult);
            return Ok(result);
        }

        [HttpGet("review-changes")]
        public async Task<IActionResult> ReviewNewChanges([FromQuery] string owner,
            [FromQuery] string repo, [FromQuery] int prNumber)
        {
            var newCodeChanges = await _pullRequestService.GetNewChangesInPR(owner, repo,
                prNumber);
            if (newCodeChanges == "No new changes found in the PR.")
                return Ok("No new code changes found for review.");

            var result = await _pullRequestService.ReviewPullRequestCode(newCodeChanges);

            if (!string.IsNullOrWhiteSpace(result))
            {
                bool isPRCommented = await _pullRequestService.PostPRComment(owner, repo, prNumber, result);
            }
            return Ok(result);
        }


    }

}
