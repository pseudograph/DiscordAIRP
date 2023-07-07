using System.Diagnostics;
using OpenAI_API.Chat;
using tavern_cli;

namespace TestProject1;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task SendThroughGateway()
    {
        var gateway = new OpenAiGateway();
        gateway.Init();
        var messages = new List<ChatMessage>
        {
            new(ChatMessageRole.User, "Hello, I'm a user.")
        };
        var result = await gateway.SendMessages(messages);
        Assert.NotNull(result);
        Console.WriteLine(result.ToString());
    }

    [Test]
    public async Task SendThroughBot()
    {
        Bot bot = new Bot("bot.json", "config.json");
        var result = await bot.SendMessage("Hello, Artoria. What is your noble phantasm?");
        Assert.NotNull(result);
        Console.WriteLine(result.ToString());
        Console.WriteLine("--------------------");
        result = await bot.SendMessage("Tell me what could potentially defeat it.");
        Assert.NotNull(result);
        Console.WriteLine(result.ToString());
    }

}