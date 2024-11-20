using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SignalRInterfaces.HubInterfaces;

namespace SignalRServer.Hubs
{
    public class MyHub : Hub, IMyHub
    {
        private ChannelReader<double>? _dataQueueChannelReader;

        private static Channel<double>? _processedDataChannel;

        public async Task SetDataQueueChannel(ChannelReader<double> dataQueueChannelReader)
        {
            _dataQueueChannelReader = dataQueueChannelReader;
            Console.WriteLine($"[INCOMING-CHANNEL] : {_dataQueueChannelReader}");
            while (await _dataQueueChannelReader.WaitToReadAsync().ConfigureAwait(false))
            {
                Console.WriteLine("[INCOMING-CHANNEL] : Processing data...");
                while (_dataQueueChannelReader.TryRead(out var data))
                {
                    Console.WriteLine($"[REQUEST-RECEIVED] : {data}");
                    if (_processedDataChannel is not null)
                    {
                        await _processedDataChannel.Writer.WriteAsync(data * 2);
                        Console.WriteLine($"[DATA-PROCESSED] : {data * 2}");
                    }
                }
            }
            Console.WriteLine("[INCOMING-CHANNEL] : Data processing done");
        }

        public ChannelReader<double> GetProcessedDataChannel()
        {
            Console.WriteLine("[OUTGOING-CHANNEL] : Requested");
            _processedDataChannel?.Writer.TryComplete();
            _processedDataChannel = Channel.CreateUnbounded<double>();
            return _processedDataChannel.Reader;
        }

        public void Close() => _processedDataChannel?.Writer.Complete();
    }
}
