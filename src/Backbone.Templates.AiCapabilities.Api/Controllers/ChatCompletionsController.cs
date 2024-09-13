using Backbone.Templates.AiCapabilities.Application.ChatCompletions.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.Templates.AiCapabilities.Api.Controllers;

[ApiController]
[Route("api/completions/chat")]
public class ChatCompletionsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async ValueTask<IActionResult> GetTextCompletionResult([FromQuery] string prompt, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTextCompletionResultCommand(prompt), cancellationToken);
        return Ok(result);
    }
}