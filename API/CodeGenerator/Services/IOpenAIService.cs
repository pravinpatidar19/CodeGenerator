namespace CodeGenerator.Services
{
    public interface IOpenAIService
    {
        Task<string> GenerateCode(string input);
        Task<string> CallOpenAIAPI(string input, string content);
    }
}
