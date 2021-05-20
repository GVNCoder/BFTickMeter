using BF3TickMeter.Data;

// ReSharper disable TypeParameterCanBeVariant

namespace BF3TickMeter.Services
{
    public interface ISettingsLoader<T>
    {
        T Load(string path);
    }
}