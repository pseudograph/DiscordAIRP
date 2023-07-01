using OpenAI_API.Chat;

namespace tavern_cli;

// CharacterCard myDeserializedClass = JsonConvert.DeserializeObject<CharacterCard>(myJsonResponse);
public class CharacterCard
{
    public string? alternate_greetings { get; set; }
    public string? avatar { get; set; }
    public object? character_book { get; set; }
    public string? character_version { get; set; }
    public string? chat { get; set; }
    public string? create_date { get; set; }
    public string? creator { get; set; }
    public string? creator_notes { get; set; }
    public string? description { get; set; }
    public Extensions? extensions { get; set; }
    public string? first_mes { get; set; }
    public string? mes_example { get; set; }
    public string? name { get; set; }
    public string? personality { get; set; }
    public string? post_history_instructions { get; set; }
    public string? scenario { get; set; }
    public string? system_prompt { get; set; }
    public List<string>? tags { get; set; }
    public string? char_greeting { get; set; }
    public string? example_dialogue { get; set; }
    public string? world_scenario { get; set; }
    public string? char_persona { get; set; }
    public string? char_name { get; set; }

    public List<ChatMessage> ToChatMessages()
    {
        var result = new List<ChatMessage>();
        result.Add(new ChatMessage(ChatMessageRole.Assistant, description));
        result.Add(new ChatMessage(ChatMessageRole.Assistant, scenario));
        result.Add(new ChatMessage(ChatMessageRole.Assistant, example_dialogue));
        return result;
    }
}

public class Chub
{
    public object? expressions { get; set; }
    public string? full_path { get; set; }
    public int id { get; set; }
    public List<object>? related_lorebooks { get; set; }
}

public class Extensions
{
    public Chub? chub { get; set; }
}

