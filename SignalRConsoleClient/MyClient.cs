using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRConsoleClient
{
    public class MyClient
    {
        private HubConnection connection;

        public async void Initialize()
        {
            connection = new HubConnectionBuilder()
                .WithUrl(new Uri("http://127.0.0.1:4444/myhub"))
                .WithKeepAliveInterval(TimeSpan.FromSeconds(5))
                .Build();
            await connection.StartAsync();
        }

        public async void SendRequest()
        {
            var channel = Channel.CreateUnbounded<double>();
            await connection.SendAsync("SetDataQueueChannel", channel.Reader).ConfigureAwait(false);

            var random = new Random();
            for (var i = 0; i < 10; i++)
            {
                var value = random.NextDouble();
                Console.WriteLine($"Sending request with value: {value}");
                await channel.Writer.WriteAsync(value).ConfigureAwait(false);
            }
            channel.Writer.Complete();
        }
    }
}
