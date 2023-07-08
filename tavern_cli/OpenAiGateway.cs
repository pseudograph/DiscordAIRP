using AI.Dev.OpenAI.GPT;
using Newtonsoft.Json;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;

namespace tavern_cli;

public class OpenAiGateway
{
    private Config? Config { get; set; }
    private OpenAIAPI? Api { get; set; }
    private Model Model { get; set; } = Model.ChatGPTTurbo;
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

    public async Task<ChatResult> SendMessages(List<ChatMessage> messages)
    {
        IsGatewayValid();
        messages.Insert(0, Config!.NsfwPrompt);
        messages.Insert(0, Config!.MainPrompt);
        messages.Add(Config!.SystemPrompt);
        var array = messages.ToArray();
        foreach (var message in array)
        {
            Console.WriteLine("[OpenAiGateway::SendMessages]: [" + message.Role + "]"
                              + "[" + message.Name + "]: " + message.Content);
        }

        
        ChatResult result = new ChatResult();
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
            catch (Exception)
            {
                Console.WriteLine("[OpenAiGateway::SendMessages]: Exception caught, retrying...");
            }
        } while (result.ToString().Length < 5);

        Console.WriteLine(result.ToString());

        return result;
    }

    public long GetPromptTokenCount()
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

    public void InitConfig(Config config)
    {
        Config = config;
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