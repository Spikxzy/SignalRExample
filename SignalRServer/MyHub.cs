using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalRServer
{
    public class MyHub : Hub
    {
        private ChannelReader<double> _dataQueueChannel;

        private Channel<double> _processedDataChannel;

        public void SetDataQueueChannel(ChannelReader<double> dataQueueChannelReader)
        {
            _dataQueueChannel = dataQueueChannelReader;
            Console.WriteLine($"Data queue received: [{_dataQueueChannel}]");
            _ = Task.Run(async () =>
            {
                Console.WriteLine("Waiting for data");
                while (await _dataQueueChannel.WaitToReadAsync().ConfigureAwait(false))
                {
                    Console.WriteLine("Processing data");
                    while (_dataQueueChannel.TryRead(out var data))
                    {
                        //await _processedDataChannel.Writer.WriteAsync(data * 2);
                        Console.WriteLine($"Data received: {data}");
                    }
                }
            });
        }

        public ChannelReader<double> GetProcessedDataChannel()
        {
            _processedDataChannel = Channel.CreateUnbounded<double>();
            return _processedDataChannel.Reader;
        }
    }
}
