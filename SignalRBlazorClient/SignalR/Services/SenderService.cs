using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR.Client;
using SignalRInterfaces.HubInterfaces;

namespace SignalRBlazorClient.SignalR.Services
{
    public class SenderService
    {
        private HubConnection _connection;

        private Channel<double> _sendDataChannel;

        public async void Initialize()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(new Uri("http://127.0.0.1:4444/myhub"))
                .WithKeepAliveInterval(TimeSpan.FromSeconds(5))
                .Build();
            await _connection.StartAsync();
            Console.WriteLine($"[SENDER-SERVICE] : Connection established with state {_connection.State}");
        }

        public async void SendServerRequest(int startVal, int stopVal)
        {
            Console.WriteLine("[SENDER-SERVICE] : Sending data to server...");
            _sendDataChannel = Channel.CreateUnbounded<double>();
            await _connection.SendAsync(nameof(IMyHub.SetDataQueueChannel), _sendDataChannel.Reader);

            var i = startVal;
            var random = new Random();

            while (i < stopVal)
            {
                var val = random.NextDouble();
                await _sendDataChannel.Writer.WriteAsync(val);
                Console.WriteLine($"[SENDER-SERVICE] : {val} sent to server");
                i++;
            }
        }

        public void FinishDataTransmission()
        {
            _sendDataChannel.Writer.Complete();
            Console.WriteLine("[SENDER-SERVICE] : Closing channel");
        }
    }
}
