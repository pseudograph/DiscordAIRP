using System.Runtime.InteropServices.JavaScript;
using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using OpenAI_API.Chat;

namespace discord_bot.Commands;

public class AiCommands : ApplicationCommandModule
{
    // This is the slash command overload for the Chat command. It is used when the slash command is called.
    // Calls the Chat function to send messages to the bot.
    [SlashCommand("chat", "Send the bot a message, including the current channel's history.")]
    public async Task Chat(InteractionContext ctx, [Option("message", "Message to send")] string message)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        tavern_cli.Bot bot = new tavern_cli.Bot(Consts.TavernBotJson, Consts.TavernConfigJson);
        var history = await ctx.Channel.GetMessagesAsync(Consts.HistoryLimit);
        var chatHistory = ConvertChatHistory(history);
        var newMessage = new ChatMessage(ChatMessageRole.User, message);
        newMessage.Name = ctx.User.Username;
        chatHistory.Add(newMessage);
        Console.WriteLine("[AiCommands::Chat(SlashCommand Overload)]: Chathistory size: " + chatHistory.Count);
        foreach (ChatMessage msg in chatHistory)
        {
            Console.WriteLine(msg.Name + ": " + msg.Content);
        }
        Console.WriteLine("[AiCommands::Chat(SlashCommand Overload)]: Sending messages.");
        var resultStringArray = await Chat(bot, ctx.User.Username, chatHistory);

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(resultStringArray[0]));
        for (int i = 1; i < resultStringArray.Count; i++)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent(resultStringArray[i]));
        }
    }
    
    // This is the event overload for the Chat command. It is used when the bot is mentioned.
    // Calls the Chat function to send messages to the bot.
    public static async Task Chat(MessageCreateEventArgs e)
    {
        tavern_cli.Bot bot = new tavern_cli.Bot(Consts.TavernBotJson, Consts.TavernConfigJson);
        var history = await e.Channel.GetMessagesAsync(Consts.HistoryLimit);
        var chatHistory = ConvertChatHistory(history);
        // Skip adding latest message to history, since it's already included.
        Console.WriteLine("[AiCommands::Chat(Event Overload)]: Chat history size: " + chatHistory.Count);
        foreach (ChatMessage msg in chatHistory)
        {
            Console.WriteLine(msg.Name + ": " + msg.Content);
        }
        Console.WriteLine("[AiCommands::Chat(Event Overload)]: Sending messages.");
        var resultStringArray = await Chat(bot, e.Author.Username, chatHistory);

        await e.Message.RespondAsync(resultStringArray[0]);
        for (int i = 1; i < resultStringArray.Count; i++)
        {
            await e.Channel.SendMessageAsync(resultStringArray[i]);
        }
    }

    // The Chat function. It takes a bot, a username, and a list of ChatMessages.
    private static async Task<List<string>> Chat(tavern_cli.Bot bot, string username, IList<ChatMessage> chatHistory)
    {
        var result = await bot.SendBulkMessages(chatHistory);
        var resultString = ReplacePlaceholderNames(result.ToString(), username, Consts.CharName);
        // forced lowercase lmeow
        resultString = resultString.ToLower();
        var resultStringArray = SplitStringByLength(resultString, 2000);
        Console.WriteLine("[AiCommands::Chat]: Result:" + resultString);
        Console.WriteLine("[AiCommands::Chat]: Result length: " + resultString.Length);
        return resultStringArray;
    }

    private static List<string> SplitStringByLength(string resultString, int chunkSize)
    {
        List<string> result = new List<string>();
        for (int i = 0; i < resultString.Length; i += chunkSize)
        {
            int length = Math.Min(chunkSize, resultString.Length - i);
            result.Add(resultString.Substring(i, length));
        }

        return result;
    }

    private static string ReplacePlaceholderNames(string result, string username, string charname)
    {
        result = result.Replace("{{char}}", charname);
        result = result.Replace("{{user}}", username);
        var resultLines = result.Split('\n');
        var trimmedResultLines = new List<string>();
        foreach (var line in resultLines)
        {
            var firstSpace = line.IndexOf(' ');
            if (firstSpace > 0 && line[firstSpace - 1] == ':') 
            {
                trimmedResultLines.Add(line.Substring(firstSpace + 1));
            }
            else
            {
                trimmedResultLines.Add(line);
            }
        }

        var trimmedResult = String.Join('\n', trimmedResultLines);
        return trimmedResult;
    }

    private static IList<ChatMessage> ConvertChatHistory(IReadOnlyList<DiscordMessage> history)
    {
        List<ChatMessage> chatHistory = new();
        // Chat history is read in reverse, and LINQ Reverse is slow.
        for (int i = history.Count - 1; i >= 0; i -= 1) 
        {
            ChatMessageRole role;
            // ASSUMES ALL BOT MESSAGES ARE FROM THE BOT!! CHANGE THIS!!
            role = history[i].Author.IsBot ? ChatMessageRole.Assistant : ChatMessageRole.User;
            var contents = history[i].Content;
            if (contents.Length == 0)
            {
                continue;
            }
            ChatMessage newMessage = new ChatMessage(role, contents);
            if (role.Equals(ChatMessageRole.User))
            {
                var username = new string(history[i].Author.Username.Where(c => !char.IsPunctuation(c)).ToArray());
                newMessage.Name = username;
            }
            else if (role.Equals(ChatMessageRole.Assistant)) newMessage.Name = Consts.CharName;
            chatHistory.Add(newMessage);
        }
        return chatHistory;
    }


}