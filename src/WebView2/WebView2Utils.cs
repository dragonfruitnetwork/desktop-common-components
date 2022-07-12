// DragonFruit Desktop Shared Components Copyright DragonFruit Network <inbox@dragonfruit.network>
// Licensed under MIT. Refer to the LICENSE file for more info

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Web.WebView2.Core;
using Serilog;

namespace DragonFruit.Software.Desktop.WebView2
{
    public static class WebView2Utils
    {
        public static async Task EnsureWebViewInstalled()
        {
            var installAttempt = 0;
            HttpClient client = null;

            // if this returns null, no webview2 environment is installed
            while (!IsWebViewInstalled())
            {
                // postfix returns value then adds 1
                if (installAttempt++ > 0)
                {
                    var result = MessageBox.Show("WebView2 install failed. try again?", "Install Failed", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result is not MessageBoxResult.Yes)
                    {
                        Environment.Exit(0);
                        return;
                    }
                }

                var downloadLocation = Path.Combine(Path.GetTempPath(), "WebView2Installer.exe");

                try
                {
                    client ??= new HttpClient();
                    await using var stream = await client.GetStreamAsync("https://go.microsoft.com/fwlink/p/?LinkId=2124703").ConfigureAwait(false);
                    await using var file = File.Create(downloadLocation);

                    await stream.CopyToAsync(file).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Log.Warning("WebView2 installer download failed: {@ex}", e);
                    continue;
                }

                if (!File.Exists(downloadLocation))
                {
                    // this'll loop round and be classed as a fail
                    continue;
                }

                var installProcess = Process.Start(new ProcessStartInfo
                {
                    FileName = downloadLocation,
                    UseShellExecute = true
                });

                Debug.Assert(installProcess is not null);
                await installProcess.WaitForExitAsync().ConfigureAwait(false);
            }

            // dispose of the webclient as it's no longer used
            client?.Dispose();
        }

        private static bool IsWebViewInstalled()
        {
            try
            {
                return CoreWebView2Environment.GetAvailableBrowserVersionString() is not null;
            }
            catch
            {
                return false;
            }
        }
    }
}
