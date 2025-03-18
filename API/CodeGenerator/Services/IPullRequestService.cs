namespace CodeGenerator.Services
{
    public interface IPullRequestService
    {
        Task<string> ReviewPullRequest(string repoOwner, string repoName, int prNumber);
        Task<string> GetNewChangesInPR(string owner, string repo, int prNumber);

        Task<string> ReviewPullRequestCode(string code);
        Task<bool> PostPRComment(string owner, string repo, int prNumber, string comment);
    }
}
