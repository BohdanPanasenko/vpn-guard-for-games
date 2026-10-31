using System;
using System.Drawing;
using System.Windows.Forms;

namespace VpnGuardForGames
{
    sealed class VpnPrompt : Form
    {
        public const string Title = "VPN Guard for Games";

        readonly Timer pollTimer = new Timer { Interval = 500 };

        public static VpnPrompt BeforeLaunch(Settings settings)
        {
            return new VpnPrompt(
                settings.Client,
                "Your VPN is connected.\n\nDisconnect it and the game will start automatically.",
                "Play anyway",
                "Cancel",
                () => !Vpn.IsConnected(settings.Adapters));
        }

        public static VpnPrompt AfterExit(Settings settings)
        {
            return new VpnPrompt(
                settings.Client,
                "The game has closed and your VPN is still off.\n\nTurn it back on so you don't forget later.",
                null,
                "Not now",
                () => Vpn.IsConnected(settings.Adapters));
        }

        VpnPrompt(string client, string text, string continueLabel, string cancelLabel, Func<bool> isResolved)
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
                Text = text
            };

            var buttons = new FlowLayoutPanel
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Margin = Padding.Empty
            };

            var cancel = CreateButton(cancelLabel, DialogResult.Cancel);
            buttons.Controls.Add(cancel);
            CancelButton = cancel;

            if (continueLabel != null)
            {
                buttons.Controls.Add(CreateButton(continueLabel, DialogResult.OK));
            }

            Button openClient = null;
            if (VpnClient.IsRunning(client))
            {
                openClient = CreateButton("Open " + client, DialogResult.None);
                openClient.Click += delegate { VpnClient.Open(client); };
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
            ActiveControl = openClient ?? buttons.Controls[buttons.Controls.Count - 1];

            pollTimer.Tick += delegate
            {
                if (isResolved())
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
