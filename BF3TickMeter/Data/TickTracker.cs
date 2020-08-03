using System;
using System.Net;
using System.Threading;
using BF3TickMeter.Services;
using PcapDotNet.Core;
using PcapDotNet.Packets;

namespace BF3TickMeter.Data
{
    public class TickTracker : ITickTracker
    {
        private const long __UpdateInterval = 1000;
        private const long __StartUpdate = 1000;

        private readonly LivePacketDevice _device;
        private readonly string _deviceAddress;
        private readonly IPEndPoint _fromEndPoint;
        private readonly IPEndPoint _toEndPoint;
        private readonly Thread _packetReadThread;
        private readonly Timer _updateTickTimer;

        private int _ticks;
        private int _maxTicks;
        private int _minTicks;

        public TickTracker(LivePacketDevice device, string deviceAddress, IPEndPoint from, IPEndPoint to)
        {
            _device = device;
            _deviceAddress = deviceAddress;
            _fromEndPoint = from;
            _toEndPoint = to;

            _ticks = 0;

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

        private void _OnUpdate(int ticks, int max, int min)
        {
            Update?.Invoke(this, new NetworkRateEventArgs(ticks, max, min));
        }

        private void _ReadPacketLoop()
        {
            while (true) // crete infinite loop
            {
                using (var communicator = _device.Open(ushort.MaxValue, PacketDeviceOpenAttributes.Promiscuous, 500))
                {
                    if (communicator.DataLink.Kind != DataLinkKind.Ethernet) return;

                    using (var filter = communicator.CreateFilter("udp"))
                        communicator.SetFilter(filter);

                    communicator.ReceivePackets(0, _PacketHandler);
                }
            }
        }

        private void _PacketHandler(Packet packet)
        {
            var packetIpV4 = packet.Ethernet.IpV4;
            var packetIpV4Address = packetIpV4.Destination.ToString();

            // catch UDP incoming packet

            var sourceIp = packetIpV4.Source.ToString();
            var destinationIp = packetIpV4.Destination.ToString();

            if (_fromEndPoint.Address.ToString() != sourceIp ||
                _toEndPoint.Address.ToString() != destinationIp) return;

            ++_ticks;
        }

        private void _UpdateTickTimer(object state)
        {
            // set max tick rate
            if (_ticks > _maxTicks) _maxTicks = _ticks;
            // set min tick rate
            if (_ticks < _minTicks && _minTicks != 0) _minTicks = _ticks;

            _OnUpdate(_ticks, _maxTicks, _minTicks);
            _ticks = 0;
        }

        #endregion
    }
}