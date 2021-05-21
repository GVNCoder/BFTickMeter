using System;

using BF3TickMeter.Data;

namespace BF3TickMeter.Services
{
    public interface ITickTracker
    {
        void Start();
        event EventHandler<NetworkRateEventArgs> Update;
    }
}