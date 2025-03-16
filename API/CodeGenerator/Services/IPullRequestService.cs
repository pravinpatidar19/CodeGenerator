using System.Threading.Channels;
using System.Threading.Tasks;
using OpenAI;

namespace CodeGenerator.Services
{
    public interface IPullRequestService
    {
        Task<string> ReviewPullRequest(string repoOwner, string repoName, int prNumber);
    }
}
