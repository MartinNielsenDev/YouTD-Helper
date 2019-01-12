using System;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
namespace YouTDHelper
{
    static class Program
    {
        public static CustomMenu customMenu;
        public static Form1 form1;
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        public static bool AutoSaveEnable = false;
        public static int AutoSaveTime = 1800;
        public static string savedata = String.Format(@"{0}\Warcraft III\CustomMapData\YouTD\savecode.txt", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        public static string ao_settingspath = String.Format(@"{0}\YouTDHelper_settings.ini", Directory.GetCurrentDirectory());
        public static Form2 ao_settingsform;
        public static List<string> ao_settings = new List<string>();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            customMenu = new CustomMenu();
            ao_settingsform = new Form2();
            form1 = new Form1();
            bool firstrun = false;
            if (!File.Exists(ao_settingspath))
            {
                firstrun = true;
            }
            else
            {
                if (!File.ReadAllText(ao_settingspath).Contains("hotkey=") || !File.ReadAllText(ao_settingspath).Contains("modifier=") || !File.ReadAllText(ao_settingspath).Contains("savecodepath=") || !File.ReadAllText(ao_settingspath).Contains("entdelay="))
                {
                    firstrun = true;
                }
            }

            Thread autosaver = new Thread(AutoSaver);
            autosaver.IsBackground = true;
            autosaver.Start();

            if (!firstrun)
            {
                string[] splitted = File.ReadAllText(ao_settingspath).Split(new string[] { "\r\n" }, StringSplitOptions.None);

                for (int i = 0; i < splitted.Length; i++)
                {
                    if (!splitted[i].Contains("hotkey=") && !splitted[i].Contains("modifier=") && !splitted[i].Contains("savecodepath=") && !splitted[i].Contains("entdelay="))
                    {
                        ao_settings.Add(splitted[i]);
                        ao_settingsform.textBox1.Text += splitted[i];
                        if (i < splitted.Length - 5)
                            ao_settingsform.textBox1.Text += "\r\n";
                    }
                    else if (splitted[i].Contains("hotkey="))
                    {
                        try { ao_settingsform.chosenkey = Convert.ToInt32(splitted[i].Split(Convert.ToChar("="))[1]); } catch { }
                    }
                    else if (splitted[i].Contains("modifier="))
                    {
                        try { ao_settingsform.chosenmodifier = Convert.ToInt32(splitted[i].Split(Convert.ToChar("="))[1]); } catch { }
                    }
                    else if (splitted[i].Contains("savecodepath="))
                    {
                        try { savedata = splitted[i].Split(Convert.ToChar("="))[1]; } catch { }
                    }
                    else if (splitted[i].Contains("entdelay="))
                    {
                        try { ao_settingsform.checkBox1.Checked = Convert.ToBoolean(splitted[i].Split(Convert.ToChar("="))[1]); } catch { }
                    }
                }
            }
            if (!File.Exists(savedata))
            {
                var firstrun_question = MessageBox.Show("I was unable to find your savecode.txt file\n\nPlease locate it by pressing OK\n\nThis will reset your settings", "YouTD Helper", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateSaveCodePath();

                string commands = "";
                string[] splitted = ao_settingsform.textBox1.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                ao_settings.Clear();

                foreach (string s in splitted)
                {
                    if (!s.Contains("hotkey=") && !s.Contains("modifier=") && !s.Contains("savecodepath="))
                    {
                        if (commands == "")
                            commands = s;
                        else
                            commands += "\r\n" + s;

                        ao_settings.Add(s);
                    }
                }
                File.WriteAllText(ao_settingspath, ao_settingsform.textBox1.Text + "\r\nhotkey=" + ao_settingsform.chosenkey + "\r\nmodifier=" + ao_settingsform.chosenmodifier + "\r\nsavecodepath=" + savedata + "\r\nentdelay=" + ao_settingsform.checkBox1.Checked);
            }
            if (firstrun)
            {
                File.WriteAllText(ao_settingspath, "-ao crit\r\n-ao sharp\r\n-ao swift\r\nhotkey=0\r\nmodifier=0\r\nsavecodepath=" + savedata + "\r\nentdelay=0");
                ao_settings.Add("-ao crit");
                ao_settingsform.textBox1.Text += "-ao crit\r\n";
                ao_settings.Add("-ao sharp");
                ao_settingsform.textBox1.Text += "-ao sharp\r\n";
                ao_settings.Add("-ao swift");
                ao_settingsform.textBox1.Text += "-ao swift";
            }
            ao_settingsform.UpdateHotkey();
            Application.Run(form1);
        }
        private static void AutoSaver()
        {
            Stopwatch timer = new Stopwatch();

            timer.Start();

            while (true)
            {
                Thread.Sleep(500);
                if (timer.ElapsedMilliseconds / 1000 > AutoSaveTime && AutoSaveEnable)
                {

                    if (GetActiveWindowTitle() == "Warcraft III")
                    {
                        NativeMethods.BlockInput(true);
                        SendKeys.SendWait("{ENTER}-save{ENTER}");
                        NativeMethods.BlockInput(false);
                        timer.Restart();
                    }
                }
            }
        }
        public static string[] GetYouTDCode()
        {
            if (!File.Exists(savedata))
            {
                MessageBox.Show("No savecode.txt file found at\n\n" + savedata, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return new string[3];
            }
            string[] result = new string[3];
            result[0] = "";
            result[1] = "";
            result[2] = "";
            var lines = File.ReadAllLines(savedata);

            foreach (string line in lines)
            {
                try
                {
                    if (line.Contains("-load"))
                    {
                        result[0] = "-load " + line.Split(new[] { "-load " }, StringSplitOptions.None)[1];
                    }
                    else if (line.Contains("Player: "))
                    {
                        result[1] = line.Split(new[] { "Player: " }, StringSplitOptions.None)[1];
                    }
                    else if (line.Contains("Game version: You TD v"))
                    {
                        result[2] = line.Split(new[] { "Game version: You TD v" }, StringSplitOptions.None)[1];
                    }
                }
                catch (Exception e) { /*MessageBox.Show(e.ToString());*/  }
            }
            return result;
        }
        public partial class NativeMethods
        {
            [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "BlockInput")]
            [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
            public static extern bool BlockInput([System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)] bool fBlockIt);
        }

        public static void BlockInput(TimeSpan span)
        {
            try
            {
                NativeMethods.BlockInput(true);
                Thread.Sleep(span);
            }
            finally
            {
                NativeMethods.BlockInput(false);
            }
        }
        public static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }
        public static string UpdateSaveCodePath()
        {
            OpenFileDialog findsavepath = new OpenFileDialog();
            findsavepath.Filter = "Text Files|savecode.txt|All Files|*";
            findsavepath.FilterIndex = 0;
            findsavepath.RestoreDirectory = true;

            if (findsavepath.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    savedata = findsavepath.FileName;
                    return savedata;
                }
                catch
                {
                }
            }
            return "";
        }
    }
}
