using System.Net;
using BF3TickMeter.Data;
using BF3TickMeter.Services;
using PcapDotNet.Core;

namespace BF3TickMeter.Helpers
{
    public static class TickTrackerHelper
    {
        public static ITickTracker BuilderTickTracker(LivePacketDevice device, IpSettings settings)
        {
            var deviceAddress = AdapterHelper.ExtractAdapterIp(device);
            var tracker = new TickTracker(device, deviceAddress, settings.ServerIPEndPoint, settings.ClientIPEndPoint);
            return tracker;
        }
    }
}