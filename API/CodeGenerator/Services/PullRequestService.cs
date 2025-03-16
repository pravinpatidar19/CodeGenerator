using System.Text;
using System.Text.Json;
using OpenAI;

namespace CodeGenerator.Services
{
    public class PullRequestService : IPullRequestService
    {
        private readonly HttpClient _httpClient;
        private readonly string _githubToken;
        private readonly string _githubUrl;
        private readonly IOpenAIService _openAIService;

        public PullRequestService(HttpClient httpClient, IConfiguration configuration, IOpenAIService openAIService)
        {
            _httpClient = httpClient;
            _githubUrl = configuration["ApiUrl:Url"];
            _githubToken = configuration["GitHub:Token"];
            _openAIService = openAIService;
        }

        public async Task<string> ReviewPullRequest(string repoOwner, string repoName, int prNumber)
        {
            var prFiles = await FetchPullRequestFiles(repoOwner, repoName, prNumber);
            if (string.IsNullOrEmpty(prFiles))
                return "No code changes found in the pull request.";

            return await AnalyzeCodeWith(prFiles);
        }

        private async Task<string> FetchPullRequestFiles(string owner, string repo, int prNumber)
        {
            //var url = $"https://api.github.com/repos/{owner}/{repo}/pulls/{prNumber}/files";
            var url = $"{_githubUrl}/{owner}/{repo}/pulls/{prNumber}/files";
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"token {_githubToken}");
            request.Headers.Add("User-Agent", "PRReviewerBot");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var files = JsonDocument.Parse(json).RootElement;

            var codeChanges = new StringBuilder();
            foreach (var file in files.EnumerateArray())
            {
                if (file.TryGetProperty("patch", out var patch))
                {
                    var fileName = file.GetProperty("filename").GetString();
                    codeChanges.AppendLine($"File: {fileName}\nChanges:\n{patch.GetString()}\n\n");
                }
            }
            return codeChanges.ToString();
        }

        private async Task<string> AnalyzeCodeWith(string code)
        {
            return await _openAIService.CallOpenAIAPI($"Please review the following code changes and provide suggestions:\n{code}",
                "You are an expert code reviewer. Provide concise suggestions and improvements.");
        }


    }
}
