using OpenAI_API.Chat;
using OpenAI_API.Models;

namespace tavern_cli;

public class Config
{
    public string ApiKey { get; set; }
    public string ServerUrl { get; set; }
    public ChatMessage MainPrompt { get; set; }
    public ChatMessage NsfwPrompt { get; set; }
    public ChatMessage SystemPrompt { get; set; }
    public Model Model { get; set; }
    public float Temperature { get; set; }
    public int? MaxTokens { get; set; }
    public float FrequencyPenalty { get; set; }
    public float PresencePenalty { get; set; }
    public Dictionary<string, float> LogitBias { get; set; } 

    public Config(string apiKey, string serverUrl, string mainPrompt, string nsfwPrompt, string systemPrompt,
        string model, float temperature, int maxtokens, float frequencyPenalty, float presencePenalty,
        Dictionary<string, float> logitBias)
    {
        ApiKey = apiKey;
        ServerUrl = serverUrl;
        MainPrompt = new ChatMessage(ChatMessageRole.System, mainPrompt);
        NsfwPrompt = new ChatMessage(ChatMessageRole.System, nsfwPrompt);
        SystemPrompt = new ChatMessage(ChatMessageRole.System, systemPrompt);
        Model = new Model(model);
        Temperature = temperature;
        MaxTokens = maxtokens;
        FrequencyPenalty = frequencyPenalty;
        PresencePenalty = presencePenalty;
        LogitBias = logitBias;
    }
}