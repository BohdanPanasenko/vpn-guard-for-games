using System;
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

            string gameCommandLine;
            CommandLine.SplitFirstToken(Environment.CommandLine, out gameCommandLine);

            if (gameCommandLine.Length == 0)
            {
                ShowStatus();
                return 0;
            }

            if (Vpn.IsConnected())
            {
                using (var prompt = new VpnPrompt())
                {
                    if (prompt.ShowDialog() != DialogResult.OK)
                    {
                        return 0;
                    }
                }
            }

            return GameLauncher.Run(gameCommandLine);
        }

        static void ShowStatus()
        {
            string status = Vpn.IsConnected() ? "connected" : "not connected";
            MessageBox.Show(
                "VPN is " + status + ".\n\nTo guard a Steam game, set its launch options to:\n\"" + Application.ExecutablePath + "\" %command%",
                VpnPrompt.Title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
