using Newtonsoft.Json;

namespace tavern_cli;


public class OpenAiRequest
{
    [JsonProperty("model")]
    public string Model { get; set; }

    [JsonProperty("prompt")]
    public string[] Prompt { get; set; }

    [JsonProperty("max_tokens")]
    public int MaxTokens { get; set; }

    public OpenAiRequest()
    {
        Model = "davinci";
        Prompt = Array.Empty<string>();
        MaxTokens = 1000;
    }
}
