using System.Text;
using discord_bot.Commands;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DSharpPlus.SlashCommands;

namespace discord_bot;

/**
 * Entry point for the program.
 * Instantiates a Bot and calls Run().GetAwaiter().GetResult() to start the bot.
 */
public static class DiscordBot
{
    public static void Main(string[] args)
    {
        var bot = new Bot();
        bot.Run().GetAwaiter().GetResult();
    }
}

/**
 * Main Bot class.
 * Instantiate this and call Run().GetAwaiter().GetResult() to start the bot.
 */
public class Bot
{
    private DiscordClient Client { get; set; }

    public Bot()
    {
        string json;
        using (var fs = File.OpenRead(Consts.DiscordConfigJson))
        using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
            json = sr.ReadToEnd();

        var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);
        
        var config = new DiscordConfiguration
        {
            Token = configJson.Token,
            TokenType = TokenType.Bot,
            AutoReconnect = true,
            MinimumLogLevel = LogLevel.Debug,
            Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
        };
        
        Client = new DiscordClient(config);
        Client.Ready += OnClientReady;
        
        var slash = Client.UseSlashCommands();

        slash.RegisterCommands<AiCommands>(configJson.TestGuild);
        slash.RegisterCommands<MiscCommands>(configJson.TestGuild);

        Client.MessageCreated += async (_, e) =>
        {
            if (DetectBotName(e, configJson)) 
            {
                await AiCommands.Chat(e, configJson);
            }
        };
    }

    /**
     * Detects messages that mention the bot by name or by @mention.
     * Ignores messages by other bots.
     * Attach this to the MessageCreated event under DiscordClient.
     */
    private bool DetectBotName(MessageCreateEventArgs e, ConfigJson configJson)
    {
        if (!e.Author.IsBot && e.Message.Content.ToLower().Contains(configJson.CharName.ToLower()))
        {
            Console.WriteLine("[DISCORD_BOT]: New message containing" + configJson.CharName + " detected.");
            return true;
        }

        if (e.MentionedUsers.Count <= 0) return false;
        foreach (var user in e.MentionedUsers)
        {
            Console.WriteLine("[DISCORD_BOT]: Mentioned user: " + user.Username);
            Console.WriteLine("[DISCORD_BOT]: Current DiscordUsername: " + configJson.DiscordUsername);
            if (e.Author.IsBot) continue;
            if (user.Username.ToLower().Contains(configJson.DiscordUsername.ToLower()))
            {
                Console.WriteLine("[DISCORD_BOT]: New mention containing" + user.Username + " detected.");
                return true;
            }
        }

        return false;    
    }

    private Task OnClientReady(DiscordClient client, ReadyEventArgs e)
    {
        return Task.CompletedTask;
    }

    public async Task Run()
    {
        await Client.ConnectAsync();
        await Task.Delay(-1);
    }
}