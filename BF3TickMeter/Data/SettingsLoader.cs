using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using BF3TickMeter.Services;

namespace BF3TickMeter.Data
{
    public class SettingsLoader : ISettingsLoader
    {
        public static ISettingsLoader CreateInstance() => new SettingsLoader();

        public void Load(out IpSettings settingsInstance, string path)
        {
            var ipStrings = _LoadStringsFromFile(path);
            var serverEndPoint = _ParseIPEndPoint(ipStrings.First());
            var clientEndPoint = _ParseIPEndPoint(ipStrings.Last());

            settingsInstance = new IpSettings
            {
                ServerIPEndPoint = serverEndPoint,
                ClientIPEndPoint = clientEndPoint
            };
        }

        #region Private methods

        private string[] _LoadStringsFromFile(string path)
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

        private IPEndPoint _ParseIPEndPoint(string raw)
        {
            var splitInput = raw.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            return new IPEndPoint(IPAddress.Parse(splitInput.First()), int.Parse(splitInput.Last()));
        }

        #endregion
    }
}