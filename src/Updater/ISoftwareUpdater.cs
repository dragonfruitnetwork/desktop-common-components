// DragonFruit Desktop Shared Components Copyright DragonFruit Network <inbox@dragonfruit.network>
// Licensed under MIT. Refer to the LICENSE file for more info

using System;
using System.Threading.Tasks;

namespace DragonFruit.Software.Desktop.Updater
{
    /// <summary>
    /// Represents a component responsible for processing application updates
    /// </summary>
    public interface ISoftwareUpdater
    {
        /// <summary>
        /// Performs a check for updates, applying them if available
        /// </summary>
        /// <param name="progressChanged">
        /// Callback used when the progress has changed.
        /// Accepts a <see cref="string"/> for textual progress updates and an <see cref="int"/> for progress bar feedback
        /// </param>
        /// <param name="allowDeltaPatches">Whether to allow delta patches to be used.</param>
        Task<bool> Perform(Action<string, int?> progressChanged, bool allowDeltaPatches = true);

        /// <summary>
        /// Schedules the client to be restarted, rebooting the latest version.
        /// </summary>
        void Restart();
    }
}
