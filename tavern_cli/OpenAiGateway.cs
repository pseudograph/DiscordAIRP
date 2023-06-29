using System.Text;
using Newtonsoft.Json;
using OpenAI_API;

namespace tavern_cli;

public class OpenAiGateway
{
    private Config? Config { get; set; }
    public OpenAIAPI? Api { get; set; }
    
    public void Init()
    {
        InitConfig();
        InitApi();
    }

    private void InitConfig()
    {
        string json = File.ReadAllText("Config.json");
        Console.WriteLine(json);
        Config = JsonConvert.DeserializeObject<Config>(json);
        if (Config == null)
        {
            throw new Exception("Config is null");
        }
    }

    private void InitApi()
    {
        Api = new OpenAIAPI(Config?.ApiKey);
        Api.ApiUrlFormat = Config?.ServerUrl;
    }
}