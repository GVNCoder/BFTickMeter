using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using BF3TickMeter.Services;
using PcapDotNet.Packets.IpV4;

namespace BF3TickMeter.Data
{
    public class SettingsLoader : ISettingsLoader
    {
        public static ISettingsLoader CreateInstance() => new SettingsLoader();

        public void Load(out IpSettings settingsInstance, string path)
        {
            var ipStrings = _LoadStringsFromFile(path);
            var sourceEndPoint = _ParseIPEndPoint(ipStrings.First());
            var destinationEndPoint = _ParseIPEndPoint(ipStrings.Last());

            settingsInstance = new IpSettings
            {
                SourceIPEndPoint = sourceEndPoint,
                DestinationIPEndPoint = destinationEndPoint
            };
        }

        #region Private methods

        private static string[] _LoadStringsFromFile(string path)
        {
            var ipStringList = new List<string>(2);

            using (var reader = File.OpenText(path))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue; // skip comments
                    
                    ipStringList.Add(line);
                }
            }

            return ipStringList.ToArray();
        }

        private static IpV4EndPoint _ParseIPEndPoint(string raw)
        {
            var splitInput = raw.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            return new IpV4EndPoint { Address = new IpV4Address(splitInput.First()), Port = uint.Parse(splitInput.Last()) };
        }

        #endregion
    }
}