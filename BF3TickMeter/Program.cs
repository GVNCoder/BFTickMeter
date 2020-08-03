using System;
using System.Collections.Generic;
using System.IO;
using BF3TickMeter.Data;
using BF3TickMeter.Helpers;
using PcapDotNet.Core;

namespace BF3TickMeter
{
    public class Program
    {
        private static readonly string __BaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xrists");

        private static void PrintAdapters(IList<LivePacketDevice> adapters)
        {
            for (var i = 0; i < adapters.Count; i++)
            {
                var livePacketDevice = adapters[i];
                Console.WriteLine($"INDEX: {i + 1} IP: {AdapterHelper.ExtractAdapterIp(livePacketDevice)} Name: {livePacketDevice.Description}");
            }
        }

        private static int SelectAdapterIndex()
        {
            Console.WriteLine();
            Console.Write("Select adapter index: ");
            var input = Console.ReadLine();

            return int.Parse(input);
        }

        public static void Main(string[] args)
        {
            // get all machine adapters
            var adapters = AdapterHelper.GetAdapters();
            // print adapters with index for select
            PrintAdapters(adapters);

            // getting adapter index from user
            var adapterUserIndex = SelectAdapterIndex();
            var adapterIndex = adapterUserIndex - 1;
            var adapter = adapters[adapterIndex];

            // getting ip settings
            var settingsLoader = SettingsLoader.CreateInstance();
            settingsLoader.Load(out var settings, __BaseDirectory);

            // clear console
            Console.Clear();

            var tracker = TickTrackerHelper.BuilderTickTracker(adapter, settings);
            tracker.Update += (sender, e) =>
            {
                // update console view here
                Console.Clear();
                Console.WriteLine($"Tickrate: Stamp: {e.CurrentRate} Max: {e.MaxRate} Min {e.MinRate}");
            };
            tracker.Start();

            Console.Read();
        }
    }
}
