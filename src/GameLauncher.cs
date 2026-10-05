using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace VpnGuardForGames
{
    static class GameLauncher
    {
        public static int Run(string commandLine)
        {
            string arguments;
            string fileName = CommandLine.SplitFirstToken(commandLine, out arguments);

            try
            {
                using (var game = Process.Start(new ProcessStartInfo(fileName, arguments) { UseShellExecute = false }))
                {
                    game.WaitForExit();
                    return game.ExitCode;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(
                    "Could not start the game.\n\n" + error.Message + "\n\nCommand:\n" + commandLine,
                    VpnPrompt.Title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return 1;
            }
        }
    }
}
