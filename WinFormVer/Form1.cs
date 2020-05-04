using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace PixivMetaWriter
{
    public partial class PixivMetaWriter : Form
    {
        public PixivMetaWriter()
        {
            InitializeComponent();
        }

        public string dirPath;
        public string savePath;
        BindingList<AllInfo> allInfos;

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFolder.ShowDialog();
            if (Directory.Exists(openFolder.SelectedPath))
            {
                textBox1.Text = openFolder.SelectedPath;
                dirPath = textBox1.Text;
                allInfos = FileOperation.GetMatchFiles(dirPath);
                if (textBox2.Text == string.Empty)
                {
                    textBox2.Text = openFolder.SelectedPath;
                    savePath = textBox2.Text;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveFolder.ShowDialog();
            textBox2.Text = saveFolder.SelectedPath;
            savePath = textBox2.Text;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)//Enter Key
            {
                if (Directory.Exists(textBox1.Text))
                {
                    dirPath = textBox1.Text;
                    allInfos = FileOperation.GetMatchFiles(dirPath);
                    textBox2.Text = dirPath;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            progressBar1.Maximum = allInfos.Count;
            progressBar1.Value = 0;
            Parallel.ForEach(allInfos, (allInfo) =>
            {
                try
                {
                    progressBar1.Value++;
                    allInfo.PixivInfo = new PixivInfo(allInfo.Pid_Page);
                }
                catch (System.Exception)
                {
                    //todo 网络异常/获取信息异常
                }
            });
            progressBar1.Value = progressBar1.Maximum;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            progressBar1.Maximum = allInfos.Count;
            progressBar1.Value = 0;

            Parallel.ForEach(allInfos, (allInfo) =>
            {
                try
                {
                    progressBar1.Value++;
                    FileOperation.ChangeInfo(allInfo);
                }
                catch (System.Exception)
                {
                    //todo 信息写入异常
                }
            });
            progressBar1.Value = progressBar1.Maximum;
        }
    }
}
