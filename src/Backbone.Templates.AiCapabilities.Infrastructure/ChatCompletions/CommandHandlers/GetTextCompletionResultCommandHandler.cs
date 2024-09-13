using System.Security.Cryptography;
using Backbone.AiCapabilities.NaturalLanguageProcessing.ChatCompletion.Abstractions.Services.Interfaces;
using Backbone.Comms.Infra.Abstractions.Commands;
using Backbone.Templates.AiCapabilities.Application.ChatCompletions.Commands;

namespace Backbone.Templates.AiCapabilities.Infrastructure.ChatCompletions.CommandHandlers;

/// <summary>
/// Command handler for getting a text completion result.
/// </summary>
public class GetTextCompletionResultCommandHandler(IChatCompletionService textCompletionService)
    : ICommandHandler<GetTextCompletionResultCommand, string>
{
    public async Task<string> Handle(GetTextCompletionResultCommand request, CancellationToken cancellationToken)
    {
        var result = await textCompletionService.SendMessageAsync(request.Prompt, cancellationToken: cancellationToken);
        return result.Content;
    }
}