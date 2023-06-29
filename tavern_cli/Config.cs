namespace tavern_cli;

public class Config
{
    public string ApiKey { get; set; }
    public string ServerUrl { get; set; }
    
    public Config(string apiKey, string serverUri)
    {
        ApiKey = apiKey;
        ServerUrl = serverUri;
    }
}