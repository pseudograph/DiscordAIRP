using System.Text;
using Newtonsoft.Json;

namespace tavern_cli;

public class OpenAiGateway
{
    private Config? Config { get; set; }
    private HttpClient? Client { get; set; }
    
    public void Init()
    {
        InitConfig();
        InitClient();
    }

    private void InitClient()
    {
        Client = new HttpClient();
        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Config?.ApiKey);
    }

    private void InitConfig()
    {
        Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("Config.json"))
                 ?? throw new InvalidOperationException("Config file invalid");
    }
    
    public async Task<string?> SendOpenAiRequest(string prompt)
    {
        if (Client == null) 
        {
            throw new InvalidOperationException("Client not initialized");
        }
            
        OpenAiRequest request = new OpenAiRequest
        {
            Prompt = new[] { prompt },
            MaxTokens = 100
        };
    
        string requestBody = JsonConvert.SerializeObject(request);
        StringContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await Client.PostAsync(Config?.ServerUri, content);
        
        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
        Console.WriteLine("Error: " + response.StatusCode);
        return null;
    }
}