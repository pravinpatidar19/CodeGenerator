using CodeGenerator.Models;
using CodeGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CodeGeneratorController : ControllerBase
    {
        private readonly IOpenAIService _openAIService;
        public CodeGeneratorController(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateCode(CodeGenerateDto codeGenerateDto)
        {
            var code = await _openAIService.GenerateCode(codeGenerateDto.Prompt);
            return Ok(code);
        }
    }

}

