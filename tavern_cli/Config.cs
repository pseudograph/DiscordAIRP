namespace tavern_cli;

public class Config
{
    public string ApiKey { get; set; }
    public string ServerUri { get; set; }
    
    public Config(string apiKey, string serverUri)
    {
        ApiKey = apiKey;
        ServerUri = serverUri;
    }
}