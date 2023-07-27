using AI.Dev.OpenAI.GPT;
using Newtonsoft.Json;
using OpenAI_API;
using OpenAI_API.Chat;

namespace tavern_cli;

/**
 * Handles communication with the OpenAI API.
 */
public class OpenAiGateway
{
    private Config? Config { get; set; }
    private OpenAIAPI? Api { get; set; }
    private long? MaxRequestTokens { get; set; }

    public OpenAiGateway(string configPath = "TavernConfig.json")
    {
        Init(configPath);
    }
    
    public void Init(string configPath = "TavernConfig.json")
    {
        InitConfig(configPath);
        InitApi();
        var usedTokenCount = GetPromptTokenCount();
        MaxRequestTokens = Config!.MaxTokens - usedTokenCount;
    }

    /**
     * Sends a List of ChatMessages to OpenAI and returns the response.
     * Inserts the Main, NSFW, and System prompts into the messages.
     * Will continue retrying until it receives a valid response.
     */
    public async Task<ChatResult?> SendMessages(List<ChatMessage> messages)
    {
        IsGatewayValid();
        messages.Insert(0, Config!.NsfwPrompt);
        messages.Insert(0, Config!.MainPrompt);
        messages.Add(Config!.SystemPrompt);

        ChatResult? result = null;
        do
        {
            try
            {
                result = await Api!.Chat.CreateChatCompletionAsync(new ChatRequest
                {
                    Model = Config!.Model,
                    Temperature = Config!.Temperature,
                    MaxTokens = Config.MaxTokens,
                    FrequencyPenalty = Config.FrequencyPenalty,
                    PresencePenalty = Config.PresencePenalty,
                    LogitBias = Config.LogitBias,
                    Messages = messages.ToArray()
                });
            }
            catch (Exception e)
            {
                Console.WriteLine("[OpenAiGateway::SendMessages]: Exception caught" + e.Message);
                Console.WriteLine("[OpenAiGateway::SendMessages]: Retrying...");
            }

            await Task.Delay(Config.RetryTimeout);
        } while (result is null || result.ToString().Length < 5);

        Console.WriteLine(result.ToString());

        return result;
    }

    /**
     * Gets the token count of the Main, NSFW, and System prompts combined.
     * This is used to calculate the MaxTokens that can be used for chat history.
     */
    private long GetPromptTokenCount()
    {
        if (Config == null)
        {
            throw new InvalidProgramException("Config is null.");
        }
        List<int> tokenCounts = GPT3Tokenizer.Encode(Config.MainPrompt.Content);
        tokenCounts.AddRange(GPT3Tokenizer.Encode(Config.NsfwPrompt.Content));
        tokenCounts.AddRange(GPT3Tokenizer.Encode(Config.SystemPrompt.Content));
        return tokenCounts.Count;
    }

    private void InitConfig(string configPath)
    {
        string json = File.ReadAllText(configPath);
        Config = JsonConvert.DeserializeObject<Config>(json)
                 ?? throw new InvalidOperationException("Config is null");
    }

    private void InitApi()
    {
        Api = new OpenAIAPI(Config!.ApiKey)
        {
            ApiUrlFormat = Config.ServerUrl
        };
    }

    private void IsGatewayValid()
    {
        if (Api == null)
        {
            throw new InvalidProgramException("[OpenAiGateway::IsGatewayValid]: API is null.");
        } if (Config == null)
        {
            throw new InvalidProgramException("[OpenAiGateway::IsGatewayValid]: Config is null.");
        }
    }
}