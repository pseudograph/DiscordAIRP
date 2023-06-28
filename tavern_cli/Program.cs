namespace tavern_cli;

static class Program
{
    private static OpenAiGateway? Gateway { get; set; }
    
    static void Main()
    {
        Gateway = new OpenAiGateway();
        Gateway.Init();
    }


}