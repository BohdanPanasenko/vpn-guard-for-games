using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VpnGuardForGames
{
    static class Program
    {
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

            bool vpnConnected = Vpn.IsConnected(settings.Adapters);

            if (vpnConnected && settings.Mode == PromptMode.Ask)
            {
                using (var prompt = new VpnPrompt(settings))
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

            using (game)
            {
                if (vpnConnected && settings.Mode == PromptMode.Warn)
                {
                    Application.Run(new VpnReminder());
                }

                game.WaitForExit();
                return game.ExitCode;
            }
        }

        static void ShowStatus(Settings settings)
        {
            string status = Vpn.IsConnected(settings.Adapters) ? "connected" : "not connected";
            MessageBox.Show(
                "VPN is " + status + ".\n\n" +
                "Mode: " + settings.Mode.ToString().ToLowerInvariant() + "\n" +
                "Settings: " + Settings.FilePath + "\n\n" +
                "To guard a Steam game, set its launch options to:\n\"" + Application.ExecutablePath + "\" %command%",
                VpnPrompt.Title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
