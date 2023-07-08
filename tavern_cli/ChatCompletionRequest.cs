using Newtonsoft.Json;

namespace tavern_cli;


/**
 * Represents a request to the OpenAI API.
 */
public class ChatCompletionRequest
{
    [JsonProperty("model")]
    public string Model { get; set; }

    [JsonProperty("messages")]
    public Message[] Messages { get; set; }

    [JsonProperty("max_tokens")]
    public int MaxTokens { get; set; }

    public ChatCompletionRequest()
    {
        Model = "gpt-3.5-turbo";
        Messages = Array.Empty<Message>();
        MaxTokens = 1000;
    }
}
public class Message
    {
        public Message(string role, string content)
        {
            Role = role;
            Content = content;
        }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }