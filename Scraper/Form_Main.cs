using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace AutoScout24
{
    public partial class Form_Main : Form
    {
        private const String PROXY_NOPROXY = "No proxy";
        private const String PROXY_HTTP = "HTTP";
        private const String PROXY_HTTPS = "HTTPS";
        private const String PROXY_SOCKS4 = "SOCKS4";
        private const String PROXY_SOCKS5 = "SOCKS5";

        public Form_Main()
        {
            InitializeComponent();
            comboBox_ProxyList.Items.Add(PROXY_NOPROXY);
            comboBox_ProxyList.Items.Add(PROXY_HTTP);
            comboBox_ProxyList.Items.Add(PROXY_HTTPS);
            comboBox_ProxyList.Items.Add(PROXY_SOCKS4);
            comboBox_ProxyList.Items.Add(PROXY_SOCKS5);
        }

        private void Form_Main_Load(object sender, EventArgs e)
        {
            String password = Config.Get(Config.KEY_PASSWORD);
            String rootDirectoryPath = Config.Get(Config.KEY_DIRECTORY);
            String dataFilename = Config.Get(Config.KEY_DATA);
            String proxyType = Config.Get(Config.KEY_PROXY_TYPE);
            String proxyAddress = Config.Get(Config.KEY_PROXY_ADDRESS);
            String proxyUsername = Config.Get(Config.KEY_PROXY_USERNAME);
            String proxyPassword = Config.Get(Config.KEY_PROXY_PASSWORD);
            textBox_Password.Text = password;
            textBox_RootDirectory.Text = rootDirectoryPath;
            textBox_DataFilename.Text = dataFilename;
            if (proxyType == null) comboBox_ProxyList.Text = PROXY_NOPROXY;
            else comboBox_ProxyList.Text = proxyType;
            textBox_ProxyAddress.Text = proxyAddress;
            textBox_ProxyUsername.Text = proxyUsername;
            textBox_ProxyPassword.Text = proxyPassword;
            comboBox_ProxyList_SelectedIndexChanged();
        }

        private Scraper scraper = null;

        private void Init()
        {
            if (scraper != null) return;
            groupBox_Proxy.Enabled = false;
            String proxyType = comboBox_ProxyList.Text;
            if (proxyType == PROXY_NOPROXY) proxyType = null;
            String proxyAddress = textBox_ProxyAddress.Text.Trim();
            String proxyUsername = textBox_ProxyUsername.Text.Trim();
            String proxyPassword = textBox_ProxyPassword.Text;
            Config.Write(Config.KEY_PROXY_TYPE, proxyType);
            Config.Write(Config.KEY_PROXY_ADDRESS, proxyAddress);
            Config.Write(Config.KEY_PROXY_USERNAME, proxyUsername);
            Config.Write(Config.KEY_PROXY_PASSWORD, proxyPassword);
            scraper = new Scraper(proxyType, proxyAddress, proxyUsername, proxyPassword);
        }

        private void button_StartPublshing_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            String password = textBox_Password.Text;
            String rootDirectoryPath = textBox_RootDirectory.Text;
            String dataFilename = textBox_DataFilename.Text;
            if (!new DirectoryInfo(rootDirectoryPath).Exists)
            {
                Scraper.Print($"Root Folder \"{rootDirectoryPath}\" does not exist.", ConsoleColor.Red);
            }
            else if (!new FileInfo(dataFilename).Exists)
            {
                Scraper.Print($"Data File \"{dataFilename}\" does not exist.", ConsoleColor.Red);
            }
            else
            {
                Config.Write(Config.KEY_PASSWORD, password);
                Config.Write(Config.KEY_DIRECTORY, rootDirectoryPath);
                Config.Write(Config.KEY_DATA, dataFilename);
                Init();
                scraper.StartPublishingThread(password, rootDirectoryPath, dataFilename);
            }
        }

        private void button_StartChecking_Click(object sender, EventArgs e)
        {
            if (new FileInfo(Scraper.LOG_FILENAME).Exists)
            {
                this.WindowState = FormWindowState.Minimized;
                String password = textBox_Password.Text;
                Config.Write(Config.KEY_PASSWORD, password);
                Init();
                scraper.StartCheckingThread(password);
            }
            else
            {
                MessageBox.Show("Log file not found.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Form_Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (scraper != null)
                scraper.Dispose();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void comboBox_ProxyList_SelectedIndexChanged(object sender = null, EventArgs e = null)
        {
            String proxyType = comboBox_ProxyList.Text;
            if (proxyType == PROXY_NOPROXY)
            {
                textBox_ProxyAddress.Enabled = false;
                textBox_ProxyUsername.Enabled = false;
                textBox_ProxyPassword.Enabled = false;
            }
            else
            {
                textBox_ProxyAddress.Enabled = true;
                textBox_ProxyUsername.Enabled = true;
                textBox_ProxyPassword.Enabled = true;
            }
        }

    }
}
