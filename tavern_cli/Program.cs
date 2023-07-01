namespace tavern_cli
{
    static class Program
    {
        private static OpenAiGateway? Gateway { get; set; }
    
        static async Task Main()
        {
            Bot bot = new Bot("bot.json", "config.json");
            while (true)
            {
                Console.WriteLine("History:");
                foreach (var message in bot.History)
                {
                    Console.WriteLine("Role: " + message.Role);
                    Console.WriteLine("Contents: " + message.Content);
                }
                Console.WriteLine("Enter message: ");
                var input = Console.ReadLine();
                if (input == null)
                {
                    Console.WriteLine("Input is null.");
                    continue;
                }
                var result = await bot.SendMessage(input);
                Console.WriteLine(result.ToString());
            }
        }
    }
}