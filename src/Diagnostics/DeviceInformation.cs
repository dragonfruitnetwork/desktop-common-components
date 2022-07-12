// DragonFruit Desktop Shared Components Copyright DragonFruit Network <inbox@dragonfruit.network>
// Licensed under MIT. Refer to the LICENSE file for more info

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace DragonFruit.Software.Desktop.Diagnostics
{
    internal static class DeviceInformation
    {
        /// <summary>
        /// Gets the commercial name of the operating system this application is running on
        /// </summary>
        public static string OperatingSystemName => SearchForOrDefault("Caption", "Win32_OperatingSystem", "Unknown OS");

        /// <summary>
        /// Gets the build number of the operating system
        /// </summary>
        public static string BuildNumber => SearchForOrDefault("BuildNumber", "Win32_OperatingSystem", "Unknown Build");

        /// <summary>
        /// Gets the OS architecture (32/64 bit)
        /// </summary>
        public static string Architecture => SearchForOrDefault("OSArchitecture", "Win32_OperatingSystem", "32-bit?");

        /// <summary>
        /// Gets the friendly name of the CPU in the current computer
        /// </summary>
        public static string ProcessorModel => SearchForOrDefault("Name", "Win32_Processor", "Unknown Processor(s)");

        /// <summary>
        /// Gets the total memory, in GB of the current computer
        /// </summary>
        public static float TotalMemory
        {
            get
            {
                try
                {
                    return SearchFor<ulong>("Capacity", "Win32_PhysicalMemory").Select(Convert.ToInt64).Sum() / 1073741824f;
                }
                catch
                {
                    return 4 * 1024;
                }
            }
        }

        /// <summary>
        /// Searches for a string, returning a fallback in case of an error
        /// </summary>
        private static string SearchForOrDefault(string key, string location, string fallback)
        {
            try
            {
                return SearchFor<string>(key, location).First();
            }
            catch
            {
                return fallback;
            }
        }

        /// <summary>
        /// Search for a key in a WMI location, returning all potential results
        /// </summary>
        private static IEnumerable<T> SearchFor<T>(string key, string location) where T : notnull
        {
            using var searcher = new ManagementObjectSearcher($"SELECT {key} FROM {location}");

            foreach (var result in searcher.Get())
            {
                yield return (T)result[key];

                result.Dispose();
            }
        }
    }
}
