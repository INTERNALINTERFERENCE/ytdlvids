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
using System.Net.Http;
using YT_downloader.Properties;
using Newtonsoft.Json.Linq;
using System.Net;
using YoutubeExplode;

namespace YT_downloader
{
    public partial class Form1 : Form
    {
        private Downloader downloader = new Downloader();
        public Form1()
        {
            InitializeComponent();
        }

        private void save_btn_Click(object sender, EventArgs e)
        {
            save_tb.Clear();
            var fbd = new FolderBrowserDialog();
            var result = fbd.ShowDialog();
            if (result == DialogResult.OK)
                save_tb.Text = fbd.SelectedPath;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                downloader.DownloadVideo1080P(link_tb.Text, progressBar1, save_tb.Text);
            }
            downloader.DownloadVideo720P(link_tb.Text, progressBar1, save_tb.Text);
        }

    }
}
