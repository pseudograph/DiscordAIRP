using System.Diagnostics;
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
        Debug.Assert(gateway.Api != null, "gateway.Api != null");
        
        var chat = gateway.Api.Chat.CreateConversation();
        // give instruction as System
        chat.AppendSystemMessage("Be a friendly internet user.");
                
        // give a few examples as user and assistant
        chat.AppendUserInput("How do you do?");
        chat.AppendUserInput("Anything interesting happening on the internet?");
                
        // and continue the conversation by asking another
        chat.AppendUserInput("Is this an animal? Chair");
        // and get another response
        var response = await chat.GetResponseFromChatbotAsync();
        Console.WriteLine(response); // "No"
        chat.AppendUserInput("Why not? Explain your answer.");
        response = await chat.GetResponseFromChatbotAsync();
        Console.WriteLine(response); // "Because it's not an animal."
        chat.AppendUserInput("What about the other questions I asked?");
        response = await chat.GetResponseFromChatbotAsync();
        Console.WriteLine(response);
    }
}