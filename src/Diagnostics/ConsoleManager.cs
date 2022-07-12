// DragonFruit Desktop Shared Components Copyright DragonFruit Network <inbox@dragonfruit.network>
// Licensed under MIT. Refer to the LICENSE file for more info

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace DragonFruit.Software.Desktop.Diagnostics
{
    [SuppressUnmanagedCodeSecurity]
    public static class ConsoleManager
    {
        private const string Kernel32DllName = "kernel32.dll";

        [DllImport(Kernel32DllName)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        private static extern bool AllocConsole();

        [DllImport(Kernel32DllName)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        private static extern bool FreeConsole();

        [DllImport(Kernel32DllName)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport(Kernel32DllName)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        private static extern int GetConsoleOutputCP();

        public static bool HasConsole => GetConsoleWindow() != IntPtr.Zero;

        /// <summary>
        /// Creates a new console instance if the process is not attached to a console already.
        /// </summary>
        public static void Show()
        {
            if (!HasConsole)
            {
                AllocConsole();
                InvalidateOutAndError();
            }
        }

        /// <summary>
        /// If the process has a console attached to it, it will be detached and no longer visible.
        /// Writing to the System.Console will still work, but no output will be shown.
        /// </summary>
        public static void Hide()
        {
            if (HasConsole)
            {
                SetOutAndErrorNull();
                FreeConsole();
            }
        }

        public static void Toggle()
        {
            if (HasConsole)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        private static void InvalidateOutAndError()
        {
            var initializeStdOutError = typeof(Console).GetMethod("InitializeStdOutError", BindingFlags.Static | BindingFlags.NonPublic);
            initializeStdOutError?.Invoke(null, new object[] { true });
        }

        private static void SetOutAndErrorNull()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }
    }
}
