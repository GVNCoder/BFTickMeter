using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PcapDotNet.Core;

namespace BF3TickMeter.Helpers
{
    public static class AdapterHelper
    {
        #region Private helpers

        private const int _SecondAdapterAddressIndex = 1;

        private static string _ExtractIpString(DeviceAddress address)
        {
            var stringAddress = address.ToString();
            var regexMatch = Regex.Match(stringAddress, "(\\d)+\\.(\\d)+\\.(\\d)+\\.(\\d)+");

            return string.IsNullOrEmpty(regexMatch.Value) ? null : regexMatch.Value;
        }

        #endregion

        public static IList<LivePacketDevice> GetAdapters()
        {
            var machineAdapters = LivePacketDevice.AllLocalMachine;
            return machineAdapters;
        }

        public static string ExtractAdapterIp(LivePacketDevice adapter)
        {
            var deviceAddress = adapter.Addresses.LastOrDefault();
            if (deviceAddress == null) return string.Empty;

            return _ExtractIpString(deviceAddress) ?? _ExtractIpString(adapter.Addresses[_SecondAdapterAddressIndex]);
        }
    }
}