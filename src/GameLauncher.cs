using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace VpnGuardForGames
{
    static class GameLauncher
    {
        const uint QueryLimitedInformation = 0x1000;

        static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(2);
        static readonly TimeSpan HandOffGracePeriod = TimeSpan.FromSeconds(10);

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(uint access, bool inheritHandle, int processId);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern bool QueryFullProcessImageName(IntPtr process, int flags, StringBuilder name, ref int size);

        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr handle);

        public static Process Start(string commandLine)
        {
            string arguments;
            string fileName = CommandLine.SplitFirstToken(commandLine, out arguments);

            try
            {
                return Process.Start(new ProcessStartInfo(fileName, arguments) { UseShellExecute = false });
            }
            catch (Exception error)
            {
                MessageBox.Show(
                    "Could not start the game.\n\n" + error.Message + "\n\nCommand:\n" + commandLine,
                    VpnPrompt.Title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return null;
            }
        }

        public static void WaitUntilClosed(Process game)
        {
            string gameFolder = GetGameFolder(game.StartInfo.FileName);
            game.WaitForExit();

            if (gameFolder == null)
            {
                return;
            }

            bool gameSeen = false;
            var sinceLastSeen = Stopwatch.StartNew();

            while (true)
            {
                if (IsAnyProcessRunningIn(gameFolder))
                {
                    gameSeen = true;
                    sinceLastSeen.Restart();
                }
                else if (gameSeen || sinceLastSeen.Elapsed >= HandOffGracePeriod)
                {
                    return;
                }

                Thread.Sleep(PollInterval);
            }
        }

        static string GetGameFolder(string fileName)
        {
            if (!Path.IsPathRooted(fileName))
            {
                return null;
            }

            string folder = Path.GetDirectoryName(Path.GetFullPath(fileName));
            string windowsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

            if (folder == null || folder == Path.GetPathRoot(folder) || IsInside(folder + "\\", windowsFolder))
            {
                return null;
            }

            return folder;
        }

        static bool IsAnyProcessRunningIn(string folder)
        {
            foreach (var process in Process.GetProcesses())
            {
                using (process)
                {
                    string path = GetImagePath(process.Id);
                    if (path != null && IsInside(path, folder))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static bool IsInside(string path, string folder)
        {
            return path.StartsWith(folder.TrimEnd('\\') + "\\", StringComparison.OrdinalIgnoreCase);
        }

        static string GetImagePath(int processId)
        {
            IntPtr handle = OpenProcess(QueryLimitedInformation, false, processId);
            if (handle == IntPtr.Zero)
            {
                return null;
            }

            try
            {
                var path = new StringBuilder(1024);
                int size = path.Capacity;
                return QueryFullProcessImageName(handle, 0, path, ref size) ? path.ToString() : null;
            }
            finally
            {
                CloseHandle(handle);
            }
        }
    }
}
