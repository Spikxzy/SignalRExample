namespace SignalRConsoleClient
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Press any key to start client");
            Console.ReadKey();

            var client = new MyClient();
            await client.Initialize();
            await client.SendRequest();

            Console.WriteLine("Press any key to close client");
            Console.ReadKey();
        }
    }
}
