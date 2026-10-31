using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VpnGuardForGames
{
    static class Program
    {
        static readonly TimeSpan MinimumPlayTimeForReconnect = TimeSpan.FromMinutes(1);

        [DllImport("user32.dll")]
        static extern bool SetProcessDPIAware();

        [STAThread]
        static int Main()
        {
            SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var settings = Settings.Load();

            string gameCommandLine;
            CommandLine.SplitFirstToken(Environment.CommandLine, out gameCommandLine);

            if (gameCommandLine.Length == 0)
            {
                ShowStatus(settings);
                return 0;
            }

            bool vpnConnectedAtLaunch = Vpn.IsConnected(settings.Adapters);

            if (vpnConnectedAtLaunch && settings.Mode == ReminderMode.Ask)
            {
                using (var prompt = VpnPrompt.BeforeLaunch(settings))
                {
                    if (prompt.ShowDialog() != DialogResult.OK)
                    {
                        return 0;
                    }
                }
            }

            Process game = GameLauncher.Start(gameCommandLine);
            if (game == null)
            {
                return 1;
            }

            var playTime = Stopwatch.StartNew();
            int exitCode;

            using (game)
            {
                if (vpnConnectedAtLaunch && settings.Mode == ReminderMode.Warn)
                {
                    Application.Run(new VpnReminder(VpnReminder.BeforeLaunchText));
                }

                game.WaitForExit();
                exitCode = game.ExitCode;
            }

            if (vpnConnectedAtLaunch && playTime.Elapsed >= MinimumPlayTimeForReconnect && !Vpn.IsConnected(settings.Adapters))
            {
                RemindToReconnect(settings);
            }

            return exitCode;
        }

        static void RemindToReconnect(Settings settings)
        {
            if (settings.Reconnect == ReminderMode.Ask)
            {
                using (var prompt = VpnPrompt.AfterExit(settings))
                {
                    prompt.ShowDialog();
                }
            }

            if (settings.Reconnect == ReminderMode.Warn)
            {
                Application.Run(new VpnReminder(VpnReminder.AfterExitText));
            }
        }

        static void ShowStatus(Settings settings)
        {
            string status = Vpn.IsConnected(settings.Adapters) ? "connected" : "not connected";
            MessageBox.Show(
                "VPN is " + status + ".\n\n" +
                "Before launch: " + settings.Mode.ToString().ToLowerInvariant() + "\n" +
                "Reconnect reminder: " + settings.Reconnect.ToString().ToLowerInvariant() + "\n" +
                "Settings: " + Settings.FilePath + "\n\n" +
                "To guard a Steam game, set its launch options to:\n\"" + Application.ExecutablePath + "\" %command%",
                VpnPrompt.Title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
