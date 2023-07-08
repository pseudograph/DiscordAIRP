using Newtonsoft.Json.Linq;
using OpenAI_API.Chat;

namespace tavern_cli;

/**
 * A bot that contains a Character Card and can communicate with the OpenAi API via an OpenAiGateway.
 */
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

    /**
     * Overload for SendMessage that takes a string instead of a ChatMessage.
     */
    public Task<ChatResult?> SendMessage(string message)
    {
        return SendMessage(new ChatMessage(ChatMessageRole.User, message));
    }

    /**
     * Sends a List of messages to OpenAI, including the CharacterCard and returns the response.
     */
    public async Task<ChatResult?> SendBulkMessages(IList<ChatMessage> messages)
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
        Console.WriteLine("[Bot::SendBulkMessages]: reply: " + reply);
        return reply;
    }

    /**
     * Sends a message to OpenAI, including the CharacterCard and returns the response.
     * Currently unused as we use SendBulkMessages along with a chatlog to simulate a chat history.
     * Use this if you want the bot to keep track of its own chat history.
     */
    private async Task<ChatResult?> SendMessage(ChatMessage message)
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
        History.Add(new ChatMessage(ChatMessageRole.Assistant, reply!.ToString()));
        return reply;
    }
}