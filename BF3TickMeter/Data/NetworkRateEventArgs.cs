using System;

namespace BF3TickMeter.Data
{
    public class NetworkRateEventArgs : EventArgs
    {
        public TickrateStamp IncomingTickrate { get; }
        public TickrateStamp OutgoingTickrate { get; }

        public NetworkRateEventArgs(TickrateStamp incomingTickrate, TickrateStamp outgoingTickrate)
        {
            IncomingTickrate = incomingTickrate;
            OutgoingTickrate = outgoingTickrate;
        }
    }
}