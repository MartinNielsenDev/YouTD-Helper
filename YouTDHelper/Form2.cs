using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace YouTDHelper
{
    public partial class Form2 : Form
    {
        public bool shift = false, ctrl = false, alt = false, changehotkey = false;
        public static int MOD_ALT = 0x1;
        public static int MOD_CONTROL = 0x2;
        public static int MOD_SHIFT = 0x4;
        public static int MOD_WIN = 0x8;
        public static int WM_HOTKEY = 0x312;
        public int chosenkey = 0, chosenmodifier = 0;
        public string currenthotkey;

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        public void UpdateHotkey()
        {
            RegisterHotKey(this.Handle, 1, chosenmodifier, chosenkey);
            RegisterHotKey(this.Handle, 0, MOD_ALT, (int)Keys.S);
        }
        protected override void WndProc(ref Message m)
        {
            var pressed = m.WParam.ToInt32();

            //Console.WriteLine(m.Msg);
            if (m.Msg == WM_HOTKEY)
            {
                UnregisterHotKey(this.Handle, 0);
                UnregisterHotKey(this.Handle, 1);

                if (Program.GetActiveWindowTitle() == "Warcraft III")
                {
                    switch (pressed)
                    {
                        case 1:
                            SendKeys.SendWait("+^%");
                            System.Threading.Thread.Sleep(1);

                            for (int i = 0; i < Program.ao_settings.Count; i++)
                            {
                                SendKeys.SendWait("{enter}");
                                SendKeys.SendWait(Program.ao_settings[i]);
                                SendKeys.SendWait("{enter}");
                                if(checkBox1.Checked)
                                {
                                    System.Threading.Thread.Sleep(200);
                                }
                            }
                            break;
                        case 0:
                            var latestcode = Program.GetYouTDCode();
                            try
                            {
                                if (File.Exists(Program.savedata + ".backups"))
                                {
                                    if (!File.ReadAllText(Program.savedata + ".backups").Contains(latestcode[0]))
                                    {
                                        File.AppendAllText(Program.savedata + ".backups", String.Format("[{2}] {1} {0}\r\n", latestcode[1], latestcode[0], File.GetLastWriteTime(Program.savedata).ToString()));
                                    }
                                }
                            }catch { }
                            SendKeys.SendWait("%");
                            System.Threading.Thread.Sleep(1);
                            SendKeys.SendWait("{enter}" + latestcode[0] + "{enter}");
                            break;
                        default:
                            //MessageBox.Show("Hotkey #" + pressed);
                            break;
                    }
                }
                RegisterHotKey(this.Handle, 0, MOD_ALT, (int)Keys.S);
                RegisterHotKey(this.Handle, 1, chosenmodifier, chosenkey);
            }

            base.WndProc(ref m);
        }
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            if (chosenmodifier == MOD_SHIFT)
            {
                currenthotkey += "SHIFT-";
            }
            else if (chosenmodifier == MOD_CONTROL)
            {
                currenthotkey += "CTRL-";
            }
            else if (chosenmodifier == MOD_ALT)
            {
                currenthotkey += "ALT-";
            }
            
            currenthotkey += ((Keys)chosenkey).ToString();
            button3.Text = Program.savedata;
            button2.Text = "Keybind: " + currenthotkey;

            button2.Focus();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                if(!changehotkey)
                    Hide();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string savecode = Program.UpdateSaveCodePath();

            if(savecode != "")
            {
                button3.Text = savecode;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            UnregisterHotKey(this.Handle, 1);
            RegisterHotKey(this.Handle, 1, chosenmodifier, chosenkey);
            string commands = "";
            string[] splitted = textBox1.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            Program.ao_settings.Clear();

            foreach (string s in splitted)
            {
                if (!s.Contains("hotkey=") && !s.Contains("modifier=") && !s.Contains("savecodepath=") && !s.Contains("entdelay="))
                {
                    if (commands == "")
                        commands = s;
                    else
                        commands += "\r\n" + s;
                    Program.ao_settings.Add(s);
                }
            }
            File.WriteAllText(Program.ao_settingspath, textBox1.Text + "\r\nhotkey=" + chosenkey + "\r\nmodifier=" + chosenmodifier + "\r\nsavecodepath=" + Program.savedata + "\r\nentdelay=" + checkBox1.Checked);
            Program.ao_settingsform.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!changehotkey)
            {
                UnregisterHotKey(this.Handle, 1);
                changehotkey = true;
                textBox1.Enabled = false;
                button1.Enabled = false;
                button3.Enabled = false;
                button2.Text = "waiting for keypress...";
            }
        }

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if (changehotkey)
            {
                if (e.KeyCode == Keys.ShiftKey)
                {
                    shift = true;
                }
                else if (e.KeyCode == Keys.ControlKey)
                {
                    ctrl = true;
                }
                else if (e.KeyCode == Keys.Menu)
                {
                    alt = true;
                }
                else
                {
                    currenthotkey = "Keybind: ";

                    if (shift)
                    {
                        chosenmodifier = MOD_SHIFT;
                        currenthotkey += "SHIFT-";
                    }
                    else if (ctrl)
                    {
                        chosenmodifier = MOD_CONTROL;
                        currenthotkey += "CTRL-";
                    }
                    else if (alt)
                    {
                        chosenmodifier = MOD_ALT;
                        currenthotkey += "ALT-";
                    }
                    else
                    {
                        chosenmodifier = 0;
                    }
                    chosenkey = (int)e.KeyCode;
                    currenthotkey += e.KeyCode;
                    button1.Enabled = true;
                    button3.Enabled = true;
                    button2.Text = currenthotkey;
                    textBox1.Enabled = true;
                    shift = false;
                    ctrl = false;
                    alt = false;
                    changehotkey = false;
                }
                // MessageBox.Show(String.Format("You pressed {0}", e.KeyCode));
            }
        }

        private void Form2_KeyUp(object sender, KeyEventArgs e)
        {
            if (changehotkey)
            {
                if (e.KeyCode == Keys.ShiftKey)
                {
                    shift = false;
                }
                else if (e.KeyCode == Keys.ControlKey)
                {
                    ctrl = false;
                }
                else if (e.KeyCode == Keys.Menu)
                {
                    alt = false;
                }
            }
        }
    }
}
