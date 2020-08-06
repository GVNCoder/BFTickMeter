using BF3TickMeter.Data;
using BF3TickMeter.Services;

using PcapDotNet.Core;
using PcapDotNet.Packets.IpV4;

namespace BF3TickMeter.Helpers
{
    public static class TickTrackerHelper
    {
        public static ITickTracker BuilderTickTracker(LivePacketDevice device, IpSettings settings)
        {
            var fromEndPoint = new IpV4Address(settings.ServerIPEndPoint.Address.ToString());
            var toEndPoint = new IpV4Address(settings.ClientIPEndPoint.Address.ToString());
            var fromPort = (uint) settings.ServerIPEndPoint.Port;
            var toPort = (uint) settings.ClientIPEndPoint.Port;


            var tracker = new TickTracker(device, fromEndPoint, fromPort, toEndPoint, toPort);
            return tracker;
        }
    }
}