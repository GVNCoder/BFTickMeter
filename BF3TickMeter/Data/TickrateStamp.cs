namespace BF3TickMeter.Data
{
    public class TickrateStamp
    {
        public int StampRate { get; }
        public int MaxRate { get; }
        public int MinRate { get; }
        public int AverageRate { get; }

        #region Overrides

        public override string ToString()
        {
            return $"stamp {StampRate} > max {MaxRate} > min {MinRate} > avg {AverageRate}";
        }

        #endregion
    }
}