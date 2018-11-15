using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace YouTDHelper
{
    public partial class CustomMenu : Form
    {
        public NotifyIcon trayIcon = new NotifyIcon();
        public ContextMenu trayMenu = new ContextMenu();
        public CustomMenu()
        {
            trayMenu.MenuItems.Add("YouTD Helper");
            trayMenu.MenuItems.Add("-");
            trayMenu.MenuItems.Add("Exit", OnExit);
            trayMenu.MenuItems[0].Enabled = false;
            trayIcon.Text = "YouTD Helper";
            trayIcon.Icon = Properties.Resources.avoidprogram_gyK_icon;
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
            trayIcon.Click += new EventHandler(openHelper);
        }
        private void openHelper(object sender, EventArgs e)
        {
            Program.form1.Show();
            Program.form1.WindowState = FormWindowState.Normal;
        }
        protected override void OnLoad(EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;

            base.OnLoad(e);
        }
        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }
        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CustomMenu
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "CustomMenu";
            this.ResumeLayout(false);

        }
    }
}
