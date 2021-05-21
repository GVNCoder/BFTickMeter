using System;
using System.IO;
using System.Collections.Generic;

using BF3TickMeter.Data;
using BF3TickMeter.Helpers;

using PcapDotNet.Core;

namespace BF3TickMeter
{
    public static class Program
    {
        private static readonly string _SettingsFilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xv");

        #region Private helper

        private static void PrintAdapters(IList<LivePacketDevice> adapters)
        {
            for (var i = 0; i < adapters.Count; i++)
            {
                var adapter = adapters[i];

                Console.WriteLine($"Index: {i + 1}\n\tDevice Ip: {AdapterHelper.ExtractAdapterIp(adapter)}\n\tDevice Name: {adapter.Description}\n");
            }
        }

        private static int SelectAdapterIndex()
        {
            Console.WriteLine();
            Console.Write("Enter adapter Index to select it: ");

            // get user input
            var input = Console.ReadLine();
            // ReSharper disable once AssignNullToNotNullAttribute
            var selectedAdapterIndex = int.Parse(input);

            return selectedAdapterIndex;
        }

        #endregion

        public static void Main(string[] args)
        {
            // get all machine adapters
            var adapters = AdapterHelper.GetAdapters();

            // print adapters with index for select
            PrintAdapters(adapters);

            try
            {
                // get adapter index from user
                var adapterUserIndex = SelectAdapterIndex();
                var adapterIndex = adapterUserIndex - 1;

                // select adapter by index
                var adapter = adapters[adapterIndex];

                // get ip settings
                var settingsLoader = SettingsLoader.CreateInstance();
                var settings = settingsLoader.Load(_SettingsFilePath);

                // clear console
                Console.Clear();

                var tracker = new TickTracker(adapter, settings);
                tracker.Update += (sender, e) =>
                {
                    // update console view here
                    Console.Clear();
                    
                    // print IpSettings
                    Console.WriteLine($"GameServer IP: {settings.GameServerEndPoint}\nClient IP: {settings.ClientEndPoint}");

                    // print tickrate stamps
                    Console.WriteLine($"Incoming tickrate: {e.IncomingTickrate}\nOutgoing tickrate: {e.OutgoingTickrate}");
                };

                tracker.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.ReadKey();
        }
    }
}
