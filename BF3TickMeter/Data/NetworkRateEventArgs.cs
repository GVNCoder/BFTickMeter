using System;

namespace BF3TickMeter.Data
{
    public class NetworkRateEventArgs : EventArgs
    {
        public int Tick { get; }
        public int MaxRate { get; }

        public NetworkRateEventArgs(int tick)
        {
            Tick = tick;
        }
    }
}