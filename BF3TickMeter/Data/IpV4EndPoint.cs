using PcapDotNet.Packets.IpV4;

namespace BF3TickMeter.Data
{
    public class IpV4EndPoint
    {
        public IpV4Address Address { get; set; }
        public uint Port { get; set; }

        public bool Equals(IpV4Address address, uint port)
            => Address == address && Port == port;
    }
}