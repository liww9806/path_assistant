using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace dir
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            try
            {
                StreamReader sr = new StreamReader("settings.txt", Encoding.Default);
                string s = sr.ReadLine();
                SelectedProj =  Convert.ToInt32(s);
                sr.Close();
            }
            catch(System.Exception)
            { }
        }

        void save_selecting()
        {
            try
            {
                StreamWriter sr = new StreamWriter("settings.txt", false, Encoding.Default);
                sr.WriteLine(Convert.ToString(SelectedProj));
                sr.Close();
            }
            catch (System.Exception)
            { }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.SelectedIndex = listBox1.SelectedIndex;
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        public struct DirItem
        {
            public string name;
            public string path;
            public DirItem(string _name, string _path)
            {
                name = _name;
                path = _path;
            }
        }
        public struct DirProj
        {
            public string name;
            public string path;
            public List<DirItem> DirItems;
            public DirProj(string _name, string _path)
            {
                name = _name;
                path = _path;
                DirItems = new List<DirItem>();
            }
        }

        public List<DirProj> DirItems = new List<DirProj>();

        string to_length(string s, int n)
        {
            string ret = s;
            for (int i = ret.Length; i < n; ++i)
            {
                ret += " ";
            }
            return ret;
        }
        string i_to_string(int i, int n)
        {
            string ret = i.ToString();
            while (ret.Length < n)
            {
                ret = "0" + ret;
            }
            return ret;
        }

        void ReadDir()
        {
            try
            {
                StreamReader sr = new StreamReader("path.txt", Encoding.Default);
                String line;
                DirProj proj = new DirProj("public", ".");
                while ((line = sr.ReadLine()) != null)
                {
                    line.Trim();
                    if (line.StartsWith("#"))
                        continue;
                    if (line.EndsWith(":"))
                    {
                        proj = new DirProj(line.Trim(':'), ".");
                        DirItems.Add(proj);
                    }
                    else
                    {
                        string[] ss = null;
                        if (line.Contains("="))
                            ss = line.Split('=');
                        else
                            ss = line.Split(';');
                        if (ss == null)
                            proj.DirItems.Add(new DirItem("null", "null"));
                        else if (ss.Length == 1)
                            proj.DirItems.Add(new DirItem(ss[0], "null"));
                        else
                            proj.DirItems.Add(new DirItem(ss[0], ss[1]));
                    }
                }
                sr.Close();

                DirProj all = new DirProj("all", ".");
                for (int i = 0; i < DirItems.Count; ++i)
                {
                    proj = DirItems[i];
                    for (int j = 0; j < proj.DirItems.Count; ++j)
                        all.DirItems.Add(proj.DirItems[j]);
                }
                DirItems.Add(all);

                for (int i = 0; i < DirItems.Count; ++i)
                {
                    proj = DirItems[i];
                    listBox3.Items.Add(proj.name);
                }

                select_proj();

                listBox3.SelectedIndex = SelectedProj;
            }
            catch(System.Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        void select_proj()
        {
            if (SelectedProj < DirItems.Count)
            {
                DirProj proj = DirItems[SelectedProj];
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                for (int i = 0; i < proj.DirItems.Count; ++i)
                {
                    DirItem item = proj.DirItems[i];
                    //listBox1.Items.Add(i_to_string(i, 2) + ". " + to_length(item.name, 20) + " ---- " + item.path);
                    listBox1.Items.Add(item.path);
                    listBox2.Items.Add(i_to_string(i, 2) + ". " + item.name);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ReadDir();
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            string path = "null";
            try
            {
                path = DirItems[SelectedProj].DirItems[listBox1.SelectedIndex].path;
                //if (CurrentMode == Mode.ModePath)
                    System.Diagnostics.Process.Start(path);
                //else
                //{
                //    string cmd = "/K \"call msvc2013-x86.bat && pushd " + path + "\"";
                //    System.Diagnostics.Process.Start("cmd.exe", cmd);
                //}
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + path);
            }
        }

        string BaseText = "路径助手 - 空格刷新 - 回车编辑";

        enum Mode
        {
            ModePath,
            ModeCmd,
            ModeMax,
        }
        //Mode CurrentMode;
        string[] ModeString = { "路径", "cmd-x86", "cmd-x64" };
        int SelectedProj = 0;

        //void ChangeMode(Mode mode)
        void refresh()
        {
            //CurrentMode = mode;
            Text = BaseText;// +ModeString[(int)mode];
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            DirItems.Clear();
            ReadDir();
        }

        private void listBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
            {
                //if (CurrentMode == Mode.ModePath)
                //    ChangeMode(Mode.ModeCmd);
                //else if (CurrentMode == Mode.ModeCmd)
                //    ChangeMode(Mode.ModePath);
                SelectedProj = listBox3.SelectedIndex;
                refresh();
                listBox3.SelectedIndex = SelectedProj;
            }
            else if (e.KeyChar == (char)13)
            {
                try
                {
                    System.Diagnostics.Process.Start("apad.exe", "path.txt");
                }
                catch (System.Exception)
                {
                    System.Diagnostics.Process.Start("notepad.exe", "path.txt");
                }
            }
            else if (e.KeyChar == (char)Keys.Escape)
                Application.Exit();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.SelectedIndex = listBox2.SelectedIndex;
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedProj = listBox3.SelectedIndex;
            save_selecting();
        }

        private void listBox3_Click(object sender, EventArgs e)
        {
            SelectedProj = listBox3.SelectedIndex;
            select_proj();
        }

        private void listBox3_DoubleClick(object sender, EventArgs e)
        {
            listBox1.SelectedIndex = 0;
            listBox1_DoubleClick(sender, e);
        }
    }
}
