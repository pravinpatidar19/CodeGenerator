using System.Net.Http.Headers;
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
        private readonly string _fileNames;

        public PullRequestService(HttpClient httpClient, IConfiguration configuration, IOpenAIService openAIService)
        {
            _httpClient = httpClient;
            _githubUrl = configuration["GitHub:Url"];
            _githubToken = configuration["GitHub:Token"];
            _openAIService = openAIService;
            _fileNames = configuration["GitHub:FileNamesToReview"];
        }

        public async Task<string> ReviewPullRequest(string repoOwner, string repoName, int prNumber)
        {
            var prFiles = await FetchPullRequestFiles(repoOwner, repoName, prNumber);


            return prFiles;
            //return await ReviewPullRequestCode(prFiles);
        }

        private async Task<string> FetchPullRequestFiles(string owner, string repo, int prNumber)
        {
            try
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
                List<string> fileNames = new List<string>();
                if (!string.IsNullOrEmpty(_fileNames))
                {
                    fileNames = _fileNames.Split(',').ToList();
                }
                var codeChanges = new StringBuilder();
                foreach (var file in files.EnumerateArray())
                {
                    var splittedFileName = file.GetProperty("filename").GetString().Split("/");
                    var fileName = splittedFileName[splittedFileName.Length - 1];
                    if (fileNames.Contains(fileName) && file.TryGetProperty("patch", out var patch))
                    {
                        codeChanges.AppendLine($"File: {fileName}\nChanges:\n{patch.GetString()}\n\n");
                    }
                }
                return codeChanges.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> ReviewPullRequestCode(string code)
        {
            return await _openAIService.CallOpenAIAPI($"Please review the following code changes and provide suggestions:\n{code}",
                "You are an expert code reviewer. Provide concise suggestions and improvements.");
        }


        public async Task<string> GetNewChangesInPR(string owner, string repo, int prNumber)
        {
            var url = $"https://api.github.com/repos/{owner}/{repo}/pulls/{prNumber}/files";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _githubToken);
            request.Headers.Add("User-Agent", "PRReviewerBot");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var files = JsonDocument.Parse(json).RootElement;

            List<string> fileNames = new List<string>();
            if (!string.IsNullOrEmpty(_fileNames))
            {
                fileNames = _fileNames.Split(',').ToList();
            }

            var codeChanges = new StringBuilder();
            foreach (var file in files.EnumerateArray())
            {
                var splittedFileName = file.GetProperty("filename").GetString().Split("/");
                var fileName = splittedFileName[splittedFileName.Length - 1];
                if (fileNames.Contains(fileName) && file.TryGetProperty("patch", out var patch))
                {
                    var newCode = ExtractAddedLines(patch.GetString());
                    if (!string.IsNullOrEmpty(newCode))
                    {
                        codeChanges.AppendLine($"File: {fileName}\n{newCode}\n\n");
                    }
                }
            }

            return codeChanges.Length > 0 ? codeChanges.ToString() : "No new changes found in the PR.";
        }

        private string ExtractAddedLines(string patch)
        {
            var addedLines = new StringBuilder();
            foreach (var line in patch.Split('\n'))
            {
                if (line.StartsWith("+") && !line.StartsWith("++")) // Ignore diff markers
                {
                    addedLines.AppendLine(line.Substring(1)); // Remove `+` prefix
                }
            }
            return addedLines.ToString();
        }

        public async Task<bool> PostPRComment(string owner, string repo, int prNumber, string comment)
        {
            var url = $"https://api.github.com/repos/{owner}/{repo}/issues/{prNumber}/comments";

            var requestBody = new { body = comment };
            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _githubToken);
            request.Headers.Add("User-Agent", "PRReviewerBot");

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        
         private void Calculation()
         {
             int value = 10;
             var result = value / 0;
         }
         
    }
}
