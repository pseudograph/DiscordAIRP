using OpenAI_API.Chat;

namespace tavern_cli;

// CharacterCard myDeserializedClass = JsonConvert.DeserializeObject<CharacterCard>(myJsonResponse);
public class CharacterCard
{
    public string? description { get; set; }
    public string? first_mes { get; set; }
    public string? mes_example { get; set; }
    public string? name { get; set; }
    public string? personality { get; set; }
    public string? scenario { get; set; }
    public string? system_prompt { get; set; }
    public string? char_greeting { get; set; }
    public string? example_dialogue { get; set; }

    public List<ChatMessage> ToChatMessages()
    {
        var result = new List<ChatMessage>();
        if (description != null)
            result.Add(new ChatMessage(ChatMessageRole.System, description));
        if (scenario != null)
            result.Add(new ChatMessage(ChatMessageRole.System, scenario));
        if (example_dialogue != null)
            result.Add(new ChatMessage(ChatMessageRole.System, example_dialogue));
        return result;
    }
}