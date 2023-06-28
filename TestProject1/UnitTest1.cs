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
        OpenAiGateway gateway = new OpenAiGateway();
        gateway.Init();
        var response = await gateway.SendOpenAiRequest("hello");
        Assert.NotNull(response);
        Console.WriteLine(response);
    }
}