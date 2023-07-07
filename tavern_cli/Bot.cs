using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

    public Bot(string botPath, string gatewayPath) {
        string json = File.ReadAllText(botPath);
        var jsonObject = JObject.Parse(json);

        CharacterCard = new CharacterCard();
        CharacterCard.description = jsonObject.SelectToken("description")?.ToString();
        CharacterCard.scenario = jsonObject.SelectToken("scenario")?.ToString();

        OpenAiGateway gateway = new OpenAiGateway();
        gateway.Init(gatewayPath);
        Gateway = gateway;
    }

    public Task<ChatResult> SendMessage(string message)
    {
        return SendMessage(new ChatMessage(ChatMessageRole.User, message));
    }

    public async Task<ChatResult> SendBulkMessages(IList<ChatMessage> messages)
    {
        if (CharacterCard == null)
        {
            throw new InvalidProgramException("[tavern_cli::SendBulkMessages]: Character card is null.");
        }
        if (Gateway == null)
        {
            throw new InvalidProgramException("[tavern_cli::SendBulkMessages]: Gateway is null.");
        }

        var payload = CharacterCard.ToChatMessages();
        payload.AddRange(messages);
        var reply = await Gateway.SendMessages(payload);
        return reply;
    }

    private async Task<ChatResult> SendMessage(ChatMessage message)
    {
        if (CharacterCard == null)
        {
            throw new InvalidProgramException("[tavern_cli::SendMessage]: Character card is null.");
        }
        if (Gateway == null)
        {
            throw new InvalidProgramException("[tavern_cli::SendMessage]: Gateway is null.");
        }
        
        History.Add(message);
        var payload = CharacterCard.ToChatMessages();
        payload.AddRange(History);
        var reply = await Gateway.SendMessages(payload);
        History.Add(new ChatMessage(ChatMessageRole.Assistant, reply.ToString()));
        return reply;
    }
}