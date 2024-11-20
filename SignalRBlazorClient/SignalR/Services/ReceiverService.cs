using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json.Linq;
using SignalRInterfaces.HubInterfaces;

namespace SignalRBlazorClient.SignalR.Services
{
    public class ReceiverService
    {
        private HubConnection _connection;

        private ChannelReader<double> _receptionChannel;

        public event EventHandler<double> DataReceived;

        public async void Initialize()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(new Uri("http://127.0.0.1:4444/myhub"))
                .WithKeepAliveInterval(TimeSpan.FromSeconds(5))
                .Build();
            await _connection.StartAsync();
            Console.WriteLine($"[RECEIVER-SERVICE] : Connection established with state {_connection.State}");
        }

        public async void StartDataReception()
        {
            _receptionChannel = await _connection.StreamAsChannelAsync<double>(nameof(IMyHub.GetProcessedDataChannel)).ConfigureAwait(false);
            Console.WriteLine($"[RECEIVER-SERVICE] : reception channel received {_receptionChannel}");
            while (await _receptionChannel.WaitToReadAsync().ConfigureAwait(false))
            {
                while (_receptionChannel.TryRead(out var value))
                {
                    DataReceived?.Invoke(this, value);
                    Console.WriteLine($"[RECEIVER-SERVICE] : {value} returned");
                }
            }
        }

        public async void FinishDataReception()
        {
            await _connection.InvokeAsync(nameof(IMyHub.Close)).ConfigureAwait(false);
            Console.WriteLine("[RECEIVER-SERVICE] : Signalized Hub to close data channel");
        }
    }
}
