using System;

namespace BF3TickMeter.Data
{
    public class NetworkRateEventArgs : EventArgs
    {
        public int CurrentRate { get; }
        public int MaxRate { get; }
        public int MinRate { get; }
        public int Average { get; }

        public NetworkRateEventArgs(int current, int max, int min, int average)
        {
            CurrentRate = current;
            MaxRate = max;
            MinRate = min;
            Average = average;
        }
    }
}