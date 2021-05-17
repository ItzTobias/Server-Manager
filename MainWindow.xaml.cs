using Server_Manager.Scripts.ServerScripts;
using Server_Manager.Viewmodels;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace Server_Manager
{
    public partial class MainWindow : Window
    {
        public static MainWindow GetMainWindow { get; private set; }
        public readonly Menu menu = new();
        public readonly ServerInfo info = new();

        public static bool WindowsTerminalExists { get; private set; } = true;

        public MainWindow()
        {
            InitializeComponent();

            FindWindowsTerminal(30);
            if (WindowsTerminalExists)
            {
                Trace.WriteLine("Windows terminal found");
            }
            else
            {
                Trace.WriteLine("Windows terminal not found");
            }

            GetMainWindow = this;
            DataContext = menu;

            ServerCollections.UpdateAll();
        }

        private static void FindWindowsTerminal(int trys)
        {
            using Process proc = new()
            {
                StartInfo = new ProcessStartInfo("wt.exe")
                {
                    CreateNoWindow = true
                }
            };

            for (int i = 0; i < trys; i++)
            {
                try
                {
                    proc.Start();
                    proc.Kill();
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(Win32Exception))
                    {
                        WindowsTerminalExists = false;
                        return;
                    }

                    if (i == trys - 1)
                    {
                        WindowsTerminalExists = false;
                    }
                }
            }
        }

        public void OpenMenu()
        {
            DataContext = menu;
        }

        public void OpenInfo(Server server)
        {
            DataContext = info;
            info.server = server;
            info.OnActivate();
        }
    }
}
