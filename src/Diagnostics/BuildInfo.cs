// DragonFruit Desktop Shared Components Copyright DragonFruit Network <inbox@dragonfruit.network>
// Licensed under MIT. Refer to the LICENSE file for more info

using System;
using System.Reflection;

namespace DragonFruit.Software.Desktop.Diagnostics
{
    public static class BuildInfo
    {
        private static readonly Version AppVersion = Assembly.GetEntryAssembly()?.GetName().Version ?? new Version();

        public static bool IsDeployedVersion => AppVersion.Major > 2;

        /// <summary>
        /// The displayable copy of <see cref="Version"/> for UI consumption
        /// </summary>
        public static readonly string DisplayVersion = IsDeployedVersion
            ? AppVersion.ToString(AppVersion.Build != 0 ? 3 : 2)
            : $"{DateTime.Now:yyyy.Mdd} (Development Edition)";
    }
}
