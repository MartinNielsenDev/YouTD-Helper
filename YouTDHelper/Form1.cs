using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace YouTDHelper
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        public static int WM_HOTKEY = 0x312;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 2;
            this.Activate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] info = Program.GetYouTDCode();
            if (info[0] != "" && info[0] != null)
            {
                textBox1.Text = info[0];
                label1.Text = "Version: " + info[2];
                label2.Text = "Player: " + info[1];
                label3.Text = "Saved at: " + File.GetLastWriteTime(Program.savedata).ToString();
                Clipboard.SetText(info[0]);

                if(File.Exists(Program.savedata + ".backups"))
                {
                    if (!File.ReadAllText(Program.savedata + ".backups").Contains(info[0]))
                    {
                        File.AppendAllText(Program.savedata + ".backups", String.Format("[{2}] {1} {0}\r\n", info[1], info[0], File.GetLastWriteTime(Program.savedata).ToString()));
                    }
                }
                button1.Text = "GET CODE";
            }
            else
            {
                button1.Text = "GET CODE - failed";
            }
        }

        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                Program.AutoSaveEnable = true;
            }
            else
            {
                Program.AutoSaveEnable = false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(comboBox1.SelectedIndex)
            {
                case 0:
                    Program.AutoSaveTime = 30;
                    break;
                case 1:
                    Program.AutoSaveTime = 60;
                    break;
                case 2:
                    Program.AutoSaveTime = 300;
                    break;
                case 3:
                    Program.AutoSaveTime = 900;
                    break;
                case 4:
                    Program.AutoSaveTime = 1800;
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Program.ao_settingsform.Show();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if(this.WindowState == FormWindowState.Minimized)
            this.Hide();
        }
    }
}
