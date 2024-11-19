using SignalRConsoleClient.Clients;

namespace SignalRConsoleClient
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var client = new MyClient();
            await client.Initialize();
            await client.SendRequest();
        }
    }
}
