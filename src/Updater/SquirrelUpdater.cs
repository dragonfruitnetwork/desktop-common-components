// DragonFruit Desktop Shared Components Copyright DragonFruit Network <inbox@dragonfruit.network>
// Licensed under MIT. Refer to the LICENSE file for more info

using System;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Squirrel;

namespace DragonFruit.Software.Desktop.Updater
{
    /// <summary>
    /// Represents a <see cref="ISoftwareUpdater"/> using Squirrel as the backend
    /// </summary>
    public class SquirrelUpdater : ISoftwareUpdater, IDisposable
    {
        private readonly UpdateManager _manager;

        public SquirrelUpdater(string appName, string url)
        {
            var isGitHub = url.Contains("github.com");
            _manager = isGitHub ? new GithubUpdateManager(url, applicationIdOverride: appName) : new UpdateManager(url, appName);
        }

        /// <inheritdoc />
        public async Task<bool> Perform(Action<string, int?> progressChanged, bool useDeltaPatching = true)
        {
            try
            {
                var info = await _manager.CheckForUpdate(!useDeltaPatching).ConfigureAwait(false);

                if (!info.ReleasesToApply.Any())
                {
                    return false;
                }

                try
                {
                    await _manager.DownloadReleases(info.ReleasesToApply, p => progressChanged.Invoke($"Downloading Update ({p}% Complete)", p)).ConfigureAwait(false);
                    await _manager.ApplyReleases(info, p => progressChanged.Invoke($"Installing ({p}% Complete)", p)).ConfigureAwait(false);

                    progressChanged.Invoke("Update Complete.", 100);

                    return true;
                }
                catch (Exception e) when (useDeltaPatching)
                {
                    Log.Warning("Error updating with deltas: {@Exception}", e);
                    await Perform(progressChanged, false).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Log.Error("Error Updating: {@Exception}", e);
                progressChanged.Invoke("Update Failed.", null);

                await Task.Delay(1250).ConfigureAwait(false);
            }

            return false;
        }

        /// <inheritdoc />
        public void Restart()
        {
            UpdateManager.RestartApp();
        }

        public void Dispose()
        {
            _manager?.Dispose();
        }
    }
}
