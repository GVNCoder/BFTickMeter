using System.CodeDom;

using PcapDotNet.Packets.IpV4;

// ReSharper disable MemberCanBePrivate.Global

namespace BF3TickMeter.Data
{
    public class IpV4EndPoint
    {
        public IpV4Address Address { get; set; }
        public uint Port { get; set; }

        #region Overrides

        public bool Equals(IpV4Address address, uint port)
            => Address == address && Port == port;

        public override string ToString()
        {
            return $"{Address}:{Port}";
        }

        #endregion
    }
}