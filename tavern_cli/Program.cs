using OpenAI_API.Chat;

namespace tavern_cli;

static class Program
{
    private static OpenAiGateway? Gateway { get; set; }
    
    static async Task Main()
    {
        Gateway = new OpenAiGateway();
        Gateway.Init();
        if (Gateway.Api == null)
        {
            throw new InvalidOperationException("Gateway.Api is null");
        }
        // the entire chat history is available in chat.Messages
    }
    
    
}