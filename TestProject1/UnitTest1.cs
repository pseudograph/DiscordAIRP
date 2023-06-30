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
    public async Task PingOpenAi()
    {
        var gateway = new OpenAiGateway();
        gateway.Init();
        var messages = new List<ChatMessage>
        {
            new(ChatMessageRole.User, "Hello, I'm a user.")
        };
        var result = await gateway.SendMessages(messages);
        Debug.WriteLine(result.ToString());
    }
}