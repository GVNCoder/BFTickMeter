using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using BF3TickMeter.Services;

using PcapDotNet.Packets.IpV4;

namespace BF3TickMeter.Data
{
    public class SettingsLoader : ISettingsLoader<IpSettings>
    {
        #region Factory method

        public static ISettingsLoader<IpSettings> CreateInstance() => new SettingsLoader();

        #endregion

        #region ISettingsLoader

        public IpSettings Load(string path)
        {
            // get ip strings from file
            var ipStrings = _ReadIpStringsFromFile(path);

            // try parse
            var settings = _ParseIpSettings(ipStrings);

            return settings;
        }

        #endregion

        #region Private methods

        private static string[] _ReadIpStringsFromFile(string filePath)
        {
            var ipStrings = new List<string>(2);

            using (var fileReader = File.OpenText(filePath))
            {
                while (! fileReader.EndOfStream)
                {
                    var contentLine = fileReader.ReadLine();

                    // validate content line
                    // if it is empty or white space or is comment (startsWith #) = skip
                    if (string.IsNullOrWhiteSpace(contentLine) || contentLine.StartsWith("#"))
                    {
                        continue;
                    }

                    ipStrings.Add(contentLine);
                }
            }

            return ipStrings.ToArray();
        }

        private static IpSettings _ParseIpSettings(IReadOnlyCollection<string> ipStrings)
        {
            if (ipStrings.Count != 2)
            {
                throw new Exception("The configuration file was not in the correct format.");
            }

            var firstEndPoint = _ParseIPEndPoint(ipStrings.First());
            var secondEndPoint = _ParseIPEndPoint(ipStrings.Last());

            return new IpSettings
            {
                GameServerEndPoint = firstEndPoint,
                ClientEndPoint = secondEndPoint
            };
        }

        private static IpV4EndPoint _ParseIPEndPoint(string raw)
        {
            var ipPortPair = raw.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            if (ipPortPair.Length != 2)
            {
                throw new Exception("The EndPoint line was not in the correct format (xxx.xxx.xxx.xxx:xxxxx).");
            }

            return new IpV4EndPoint
            {
                Address = new IpV4Address(ipPortPair.First()),
                Port = uint.Parse(ipPortPair.Last())
            };
        }

        #endregion
    }
}