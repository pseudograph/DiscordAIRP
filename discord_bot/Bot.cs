using System.Text;
using discord_bot.Commands;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DSharpPlus.SlashCommands;

namespace discord_bot;

public class DiscordBot
{
    public static void Main(string[] args)
    {
        var bot = new Bot();
        bot.Run().GetAwaiter().GetResult();
    }
}

public class Bot
{
    public DiscordClient Client { get; private set; }

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

        slash.RegisterCommands<AiCommands>(Consts.TestGuild);
        slash.RegisterCommands<MiscCommands>(Consts.TestGuild);

        Client.MessageCreated += async (s, e) =>
        {
            if (DetectBotName(s, e)) 
            {
                await AiCommands.Chat(e);
            }
        };
    }

    private bool DetectBotName(DiscordClient s, MessageCreateEventArgs e)
    {
        if (!e.Author.IsBot && e.Message.Content.ToLower().Contains(Consts.CharName.ToLower()))
        {
            Console.WriteLine("[DISCORD_BOT]: New message containing" + Consts.CharName + " detected.");
            return true;
        }

        if (e.MentionedUsers.Count <= 0) return false;
        foreach (var user in e.MentionedUsers)
        {
            Console.WriteLine("[DISCORD_BOT]: Mentioned user: " + user.Username);
            Console.WriteLine("[DISCORD_BOT]: Current DiscordUsername: " + Consts.DiscordUsername);
            if (e.Author.IsBot) continue;
            if (user.Username.ToLower().Contains(Consts.DiscordUsername.ToLower()))
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