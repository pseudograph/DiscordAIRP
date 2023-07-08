using Newtonsoft.Json;

namespace discord_bot;

public struct ConfigJson
{
    [JsonProperty("Token")]
    public string Token { get; set; }
    [JsonProperty("Prefix")]
    public string Prefix { get; set; }
    [JsonProperty("TestGuild")] 
    public ulong TestGuild { get; set;  }
    [JsonProperty("TavernConfigJson")] 
    public string TavernConfigJson { get; set; }
    [JsonProperty("TavernBotJson")] 
    public string TavernBotJson { get; set; }
    [JsonProperty("CharName")] 
    public string CharName { get; set; }
    [JsonProperty("DiscordUsername")] 
    public string DiscordUsername { get; set; }
    [JsonProperty("HistoryLimit")] 
    public int HistoryLimit { get; set; }
}