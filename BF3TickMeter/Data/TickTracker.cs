using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using BF3TickMeter.Services;

using PcapDotNet.Core;
using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;

namespace BF3TickMeter.Data
{
    public class TickTracker : ITickTracker
    {
        private const int __UpdateInterval = 1000;
        private const int __StartUpdate = 1000;
        private const int __AverageBufferSize = 100;

        private readonly LivePacketDevice _device;
        private readonly Thread _packetReadThread;
        private readonly Timer _updateTickTimer;
        private readonly List<int> _averageBuffer;
        private readonly IpV4EndPoint _dstEndPoint;
        private readonly IpV4EndPoint _srcEndPoint;

        private int _ticks;
        private int _maxTicks;
        private int _minTicks;

        public TickTracker(LivePacketDevice device, IpSettings settings)
        {
            _device = device;

            _dstEndPoint = settings.DestinationIPEndPoint;
            _srcEndPoint = settings.SourceIPEndPoint;

            _ticks = 0;
            _maxTicks = 0;
            _minTicks = 0;
            _averageBuffer = new List<int>(__AverageBufferSize);

            _packetReadThread = new Thread(_ReadPacketLoop);
            _updateTickTimer = new Timer(_UpdateTickTimer, null, Timeout.Infinite, __UpdateInterval);
        }

        public void Start()
        {
            _packetReadThread.Start();
            _updateTickTimer.Change(__StartUpdate, __UpdateInterval);
        }

        public event EventHandler<NetworkRateEventArgs> Update;

        #region Private methods

        private void _OnUpdate(int ticks, int max, int min, int average)
        {
            Update?.Invoke(this, new NetworkRateEventArgs(ticks, max, min, average));
        }

        private void _ReadPacketLoop()
        {
            while (true) // crete infinite loop
            {
                using (var communicator = _device.Open(ushort.MaxValue, PacketDeviceOpenAttributes.Promiscuous, 500))
                {
                    if (communicator.DataLink.Kind != DataLinkKind.Ethernet) return;

                    // setup packet filter
                    using (var filter = communicator.CreateFilter("udp"))
                        communicator.SetFilter(filter);
                    // begin receive packets
                    communicator.ReceivePackets(0, _PacketHandler);
                }
            }
        }

        private void _PacketHandler(Packet packet)
        {
            // split incoming useful data
            var incIPv4Payload = packet.Ethernet.IpV4;
            var incProtoDatagram = incIPv4Payload.Udp;

            var dstIp = incIPv4Payload.Destination;
            var srcIp = incIPv4Payload.Source;

            var dstPort = incProtoDatagram.DestinationPort;
            var srcPort = incProtoDatagram.SourcePort;

            // filter packets
            if (_PassPacket(dstIp, dstPort, srcIp, srcPort)) ++_ticks;
        }

        private bool _PassPacket(IpV4Address dstAddress, uint dstPort, IpV4Address srcAddress, uint srcPort)
            => _dstEndPoint.Equals(dstAddress, dstPort) && _srcEndPoint.Equals(srcAddress, srcPort);

        private void _UpdateTickTimer(object state)
        {
            // calculate average count of ticks
            var buffCount = _averageBuffer.Count;
            if (buffCount == __AverageBufferSize)
                _averageBuffer.RemoveAt(0);

            // add ticks stamp to average buff
            _averageBuffer.Add(_ticks);
            
            var tickSum = _averageBuffer.Sum();
            var average = tickSum == 0 || buffCount == 0
                ? 0
                : tickSum / (buffCount < __AverageBufferSize ? buffCount : __AverageBufferSize);

            // set max tick rate
            if (_ticks > _maxTicks) _maxTicks = _ticks;
            // set min tick rate
            if (_ticks < _minTicks || _minTicks == 0) _minTicks = _ticks;

            _OnUpdate(_ticks, _maxTicks, _minTicks, average);
            _ticks = 0;
        }

        #endregion
    }
}