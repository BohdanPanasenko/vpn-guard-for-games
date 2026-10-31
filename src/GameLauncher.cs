using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace VpnGuardForGames
{
    static class GameLauncher
    {
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
    }
}
