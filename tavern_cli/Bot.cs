using Newtonsoft.Json;
using OpenAI_API.Chat;

namespace tavern_cli;

public class Bot
{
    private OpenAiGateway Gateway { get; set; }
    private CharacterCard CharacterCard { get; set; }
    public List<ChatMessage> History { get; set; } = new();
    
    // Bot params
    public Bot(CharacterCard characterCard, OpenAiGateway gateway)
    {
        Gateway = gateway;
        CharacterCard = characterCard;
    }

    public Bot(string botPath, string gatewayPath)
    {
        string json = File.ReadAllText(botPath);
        CharacterCard = JsonConvert.DeserializeObject<CharacterCard>(json)
                 ?? throw new InvalidOperationException("CharacterCard is null");

        OpenAiGateway gateway = new OpenAiGateway();
        gateway.Init(gatewayPath);
        Gateway = gateway;
    }

    public Task<ChatResult> SendMessage(string message)
    {
        return SendMessage(new ChatMessage(ChatMessageRole.User, message));
    }

    private async Task<ChatResult> SendMessage(ChatMessage message)
    {
        if (CharacterCard == null)
        {
            throw new InvalidProgramException("Character card is null.");
        }
        if (Gateway == null)
        {
            throw new InvalidProgramException("Gateway is null.");
        }
        
        History.Add(message);
        var payload = CharacterCard.ToChatMessages();
        payload.AddRange(History);
        var reply = await Gateway.SendMessages(payload);
        History.Add(new ChatMessage(ChatMessageRole.Assistant, reply.ToString()));
        return reply;
    }
    
}