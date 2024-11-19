using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using SignalRInterfaces.HubInterfaces;

namespace SignalRConsoleClient.Clients
{
    public class MyClient
    {
        private HubConnection connection;

        public async Task Initialize()
        {
            Console.WriteLine("[CONNECTION] : Initializing...");
            connection = new HubConnectionBuilder()
                .WithUrl(new Uri("http://127.0.0.1:4444/myhub"))
                .WithKeepAliveInterval(TimeSpan.FromSeconds(5))
                .Build();
            await connection.StartAsync();
            Console.WriteLine("[CONNECTION] : Initialized");
        }

        public async Task SendRequest()
        {
            Console.WriteLine("Press ENTER to start the data streaming process...");
            Console.ReadLine();

            var resultChannel = await connection.StreamAsChannelAsync<double>(nameof(IMyHub.GetProcessedDataChannel)).ConfigureAwait(false);

            _ = Task.Factory.StartNew(async () =>
            {
                Console.WriteLine("[CHANNEL] : Waiting to read...");
                while (await resultChannel.WaitToReadAsync().ConfigureAwait(false))
                {
                    Console.WriteLine("[CHANNEL] : Reading data...");
                    while (resultChannel.TryRead(out var channelResult))
                    {
                        Console.WriteLine($"[CHANNEL] : Result: {channelResult}");
                    }

                    Console.WriteLine("[CHANNEL] : Done");
                }
            }, TaskCreationOptions.LongRunning);

            var channel = Channel.CreateUnbounded<double>();
            await connection.SendAsync(nameof(IMyHub.SetDataQueueChannel), channel.Reader).ConfigureAwait(false);

            var random = new Random();
            for (var i = 0; i < 10; i++)
            {
                var value = random.NextDouble();
                Console.WriteLine($"[SENDING REQUEST] : {value}");
                await channel.Writer.WriteAsync(value).ConfigureAwait(false);
            }

            Console.WriteLine("[TRANSMISSION DONE] : Press ENTER to close channels...");
            Console.ReadLine();

            channel.Writer.Complete();
            await connection.InvokeAsync(nameof(IMyHub.Close)).ConfigureAwait(false);

            Console.WriteLine("[DONE] : Press ENTER to exit...");
            Console.ReadLine();
        }
    }
}
