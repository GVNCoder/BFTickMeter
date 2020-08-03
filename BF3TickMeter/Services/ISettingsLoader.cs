using BF3TickMeter.Data;

namespace BF3TickMeter.Services
{
    public interface ISettingsLoader
    {
        void Load(out IpSettings instance, string path);
    }
}