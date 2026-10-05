using System.Drawing;
using System.Windows.Forms;

namespace VpnGuardForGames
{
    sealed class VpnPrompt : Form
    {
        public const string Title = "VPN Guard for Games";

        readonly Timer pollTimer = new Timer { Interval = 500 };

        public VpnPrompt(Settings settings)
        {
            Text = Title;
            Font = SystemFonts.MessageBoxFont;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            TopMost = true;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Padding = new Padding(16);

            var message = new Label
            {
                AutoSize = true,
                MaximumSize = new Size(380, 0),
                Margin = new Padding(0, 0, 0, 16),
                Text = "Your VPN is connected.\n\nDisconnect it and the game will start automatically."
            };

            var buttons = new FlowLayoutPanel
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Margin = Padding.Empty
            };

            var cancel = CreateButton("Cancel", DialogResult.Cancel);
            var playAnyway = CreateButton("Play anyway", DialogResult.OK);
            buttons.Controls.Add(cancel);
            buttons.Controls.Add(playAnyway);
            CancelButton = cancel;

            Button openClient = null;
            if (VpnClient.IsRunning(settings.Client))
            {
                openClient = CreateButton("Open " + settings.Client, DialogResult.None);
                openClient.Click += delegate { VpnClient.Open(settings.Client); };
                buttons.Controls.Add(openClient);
            }

            var layout = new TableLayoutPanel
            {
                AutoSize = true,
                ColumnCount = 1,
                Dock = DockStyle.Fill
            };
            layout.Controls.Add(message);
            layout.Controls.Add(buttons);
            Controls.Add(layout);
            ActiveControl = openClient ?? playAnyway;

            pollTimer.Tick += delegate
            {
                if (!Vpn.IsConnected(settings.Adapters))
                {
                    DialogResult = DialogResult.OK;
                }
            };

            Shown += delegate
            {
                Activate();
                pollTimer.Start();
            };
            FormClosed += delegate { pollTimer.Dispose(); };
        }

        static Button CreateButton(string text, DialogResult result)
        {
            return new Button
            {
                Text = text,
                DialogResult = result,
                AutoSize = true,
                MinimumSize = new Size(96, 0),
                Padding = new Padding(8, 2, 8, 2)
            };
        }
    }
}
