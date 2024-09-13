using Backbone.Comms.Infra.Abstractions.Commands;

namespace Backbone.Templates.AiCapabilities.Application.ChatCompletions.Commands;

/// <summary>
/// Represents a command to get a text completion result.
/// </summary>
/// <param name="Prompt">The prompt as input</param>
public sealed record GetTextCompletionResultCommand(string Prompt) : ICommand<string>;