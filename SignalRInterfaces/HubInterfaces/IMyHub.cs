using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SignalRInterfaces.HubInterfaces
{
    public interface IMyHub
    {
        public Task SetDataQueueChannel(ChannelReader<double> dataQueueChannelReader);

        public ChannelReader<double> GetProcessedDataChannel();

        public void Close();
    }
}
