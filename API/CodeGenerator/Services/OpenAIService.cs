using System.Text;
using System.Text.Json;
using OpenAI;
using OpenAI.Chat;

namespace CodeGenerator.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly string _apiKey;
        private readonly string _apiUrl;
        private readonly string _deploymentName;
        private readonly HttpClient _httpClient;

        public OpenAIService(HttpClient httpClient, IConfiguration configuration)
        {
            _apiKey = configuration["OpenAI:ApiKey"];
            _apiUrl = configuration["OpenAI:ApiUrl"];
            _deploymentName = configuration["OpenAI:DeploymentName"];
            _httpClient = httpClient;
        }

        public async Task<string> GenerateCode(string input)
        {
            return await CallOpenAIAPI(input, "You are highly efficient code generator AI.");
        }

        public async Task<string> CallOpenAIAPI(string input, string content)
        {
            try
            {
                var requestUri = $"{_apiUrl}/openai/deployments/{_deploymentName}/chat/completions?api-version=2024-08-01-preview";
                var request = new
                {
                    //model = "gpt-4o-mini",
                    messages = new[]
                    {
                        new { role = "system", content = content },
                        new { role = "user", content = input }
                    },
                    temperature = 0.7,
                    max_tokens = 100,
                    top_p = 1.0,
                    frequency_penalty = 0.0,
                    presence_penalty = 0.0
                };

                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
                httpRequestMessage.Headers.Add("api-key", _apiKey);
                httpRequestMessage.Content = JsonContent.Create(request);

                var response = await _httpClient.SendAsync(httpRequestMessage);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadFromJsonAsync<JsonElement>();
                return responseContent.GetProperty("choices").EnumerateArray().First().GetProperty("message").GetProperty("content").ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //public void CallOpenAIAPI()
        //{
        //    var openAIClient = new OpenAIClient(_apiKey);
        //    var chatCompletionsOptions = new ChatCompletionOptions();
        //    var chatMessage = new ChatMessage
        //    {
        //        Content = "Hello, how are you?"
        //    };
        //    chatCompletionsOptions.Messages.Add(chatMessage);
        //    var response = openAIClient.GetChatCompletionsAsync(_deploymentName, chatCompletionsOptions);
        //    return response.Result;
        //}
    }
}
