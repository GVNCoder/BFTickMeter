using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using BF3TickMeter.Services;
using PcapDotNet.Core;
using PcapDotNet.Packets;

namespace BF3TickMeter.Data
{
    public class TickTracker : ITickTracker
    {
        private const int __UpdateInterval = 1000;
        private const int __StartUpdate = 1000;
        private const int __AverageBufferSize = 100;

        private readonly LivePacketDevice _device;
        private readonly string _deviceAddress;
        private readonly IPEndPoint _fromEndPoint;
        private readonly IPEndPoint _toEndPoint;
        private readonly Thread _packetReadThread;
        private readonly Timer _updateTickTimer;

        private int _ticks;
        private int _maxTicks;
        private int _minTicks;
        private List<int> _averageBuffer;

        public TickTracker(LivePacketDevice device, string deviceAddress, IPEndPoint from, IPEndPoint to)
        {
            _device = device;
            _deviceAddress = deviceAddress;
            _fromEndPoint = from;
            _toEndPoint = to;

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
            var packetIpV4 = packet.Ethernet.IpV4;
            var packetIpV4Address = packetIpV4.Destination.ToString();

            // catch incoming packet

            var sourceIp = packetIpV4.Source.ToString();
            var destinationIp = packetIpV4.Destination.ToString();

            // filter incoming packets by IP`s
            if (_fromEndPoint.Address.ToString() != sourceIp ||
                _toEndPoint.Address.ToString() != destinationIp) return;

            ++_ticks;
        }

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