using Server_Manager.Scripts.Initialization;
using Server_Manager.Scripts.ServerScripts;
using Server_Manager.UIElements;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace Server_Manager
{
    public partial class MainWindow : Window
    {
        public static MainWindow GetMainWindow { get; private set; }
        public Menu Menu { get; }
        public ServerInfo Info { get; }

        public static bool WindowsTerminalExists { get; private set; } = true;

        public MainWindow()
        {
            InitializeComponent();

            Menu = new Menu();
            //Info = new ServerInfo();

            //FindWindowsTerminal(30);
            //if (WindowsTerminalExists)
            //{
            //    Trace.WriteLine("Windows terminal found");
            //}
            //else
            //{
            //    Trace.WriteLine("Windows terminal not found");
            //}

            StateChanged += WindowStateChanged;
            GetMainWindow = this;
            DataContext = Menu;

            Loaded += (object sender, RoutedEventArgs e) =>
            {
                Initializer.Initialize();
            };
        }

        private void WindowStateChanged(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    BorderThickness = new Thickness(0);
                    break;
                case WindowState.Maximized:
                    BorderThickness = new Thickness(5);
                    break;
                default:
                    break;
            }
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
            DataContext = Menu;
        }

        public void OpenInfo(Server server)
        {
            DataContext = Info;
            Info.server = server;
            Info.OnActivate();
        }

        public void Minimize()
        {
            WindowState = WindowState.Minimized;
        }
    }
}
