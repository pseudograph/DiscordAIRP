using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace discord_bot.Commands;

public class MiscCommands : ApplicationCommandModule
{
    [SlashCommand("ping", "responds with ping")]
    public async Task Ping(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent("Pong"));
    }
}