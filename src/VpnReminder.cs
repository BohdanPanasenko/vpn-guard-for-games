using System.Drawing;
using System.Windows.Forms;

namespace VpnGuardForGames
{
    sealed class VpnReminder : Form
    {
        const int VisibleMs = 8000;
        const int ScreenMargin = 16;
        const int TopMostStyle = 0x8;
        const int ToolWindowStyle = 0x80;
        const int NoActivateStyle = 0x8000000;

        readonly Timer closeTimer = new Timer { Interval = VisibleMs };

        public VpnReminder()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Padding = new Padding(16, 12, 16, 12);
            BackColor = Color.FromArgb(32, 32, 32);
            ForeColor = Color.White;
            Font = SystemFonts.MessageBoxFont;
            Cursor = Cursors.Hand;

            var message = new Label
            {
                AutoSize = true,
                Margin = Padding.Empty,
                Text = "Your VPN is connected, expect high ping.\nClick to dismiss."
            };
            message.Click += delegate { Close(); };
            Click += delegate { Close(); };
            Controls.Add(message);

            Load += delegate
            {
                PerformLayout();
                Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
                Location = new Point(workingArea.Right - Width - ScreenMargin, workingArea.Bottom - Height - ScreenMargin);
            };
            closeTimer.Tick += delegate { Close(); };
            Shown += delegate { closeTimer.Start(); };
            FormClosed += delegate { closeTimer.Dispose(); };
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams parameters = base.CreateParams;
                parameters.ExStyle |= TopMostStyle | ToolWindowStyle | NoActivateStyle;
                return parameters;
            }
        }
    }
}
