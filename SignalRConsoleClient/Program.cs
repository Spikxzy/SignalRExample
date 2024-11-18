namespace SignalRConsoleClient
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start client");
            Console.ReadKey();

            var client = new MyClient();
            client.Initialize();
            client.SendRequest();

            Console.WriteLine("Press any key to close client");
            Console.ReadKey();
        }
    }
}
