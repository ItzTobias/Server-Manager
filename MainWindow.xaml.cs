using Server_Manager.Scripts;
using Server_Manager.Scripts.Initialization;
using Server_Manager.UIElements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Server_Manager
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        //private List<UserControl>

        private readonly TextBlock errorHeader = new()
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 18,
            Margin = new Thickness(15, 0, 15, 0),
            Foreground = App.RedBrush,
            FontWeight = FontWeights.Bold
        };
        private bool isDisplayingErrorMessage;
        private object oldControl;

        public event PropertyChangedEventHandler PropertyChanged;
        public object CurrentControl { get; private set; }

        public static MainWindow GetMainWindow { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            StateChanged += WindowStateChanged;
            GetMainWindow = this;
            DockPanel.SetDock(errorHeader, Dock.Left);

            ChangeCurrentControl(new ServerList());

            Loaded += Initializer.Initialize;

            /*
            FindWindowsTerminal(30);
            if (WindowsTerminalExists)
            {
                Trace.WriteLine("Windows terminal found");
            }
            else
            {
                Trace.WriteLine("Windows terminal not found");
            }
            */
        }

        public void ChangeCurrentControl(object newControl)
        {
            if (isDisplayingErrorMessage)
            {
                return;
            }

            TopMenuItemPanel.Children.RemoveRange(4, TopMenuItemPanel.Children.Count - 1);

            CurrentControl = newControl;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentControl)));

            if (CurrentControl is not IHasTopMenuItems)
            {
                return;
            }

            IHasTopMenuItems topMenuItems = CurrentControl as IHasTopMenuItems;

            foreach (UIElement item in topMenuItems.Items)
            {
                item.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Left);
                TopMenuItemPanel.Children.Add(item);
                DockPanel.SetDock(item, Dock.Left);
            }
        }
        public void ShowErrorMessage(ErrorMessage message)
        {
            if (isDisplayingErrorMessage)
            {
                return;
            }
            isDisplayingErrorMessage = true;

            oldControl = CurrentControl;
            CurrentControl = message;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentControl)));

            TopMenuItemPanel.Children.RemoveRange(4, TopMenuItemPanel.Children.Count - 1);

            errorHeader.Text = message.Text;

            TopMenuItemPanel.Children.Add(errorHeader);

            SettingsButton.IsEnabled = false;
            SettingsButton.Visibility = Visibility.Hidden;
        }
        public void HideErrorMessage()
        {
            if (!isDisplayingErrorMessage)
            {
                return;
            }
            isDisplayingErrorMessage = false;

            ChangeCurrentControl(oldControl);
            oldControl = null;

            SettingsButton.IsEnabled = true;
            SettingsButton.Visibility = Visibility.Visible;
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
        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                Point mousePosition = e.GetPosition(this);
                double relativeXPosition = mousePosition.X / ActualWidth;

                double finalXPosition = 0;
                double screenWidth = 0;
                if (relativeXPosition < .5)
                {
                    finalXPosition = 0;
                }
                else
                {
                    screenWidth = ActualWidth;
                }

                MaximizeRestoreButton.ChangeState(this, new RoutedEventArgs(e.RoutedEvent, sender));

                double halfWindowWidth = ActualWidth / 2;
                if (relativeXPosition < .5)
                {
                    if (mousePosition.X > halfWindowWidth)
                    {
                        finalXPosition = mousePosition.X - halfWindowWidth;
                    }
                }
                else
                {
                    finalXPosition = screenWidth - ActualWidth;

                    if (mousePosition.X < halfWindowWidth + finalXPosition)
                    {
                        finalXPosition = mousePosition.X - halfWindowWidth;
                    }
                }

                Left = finalXPosition;
                Top = 0;
            }

            DragMove();
        }
        private void Close(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Minimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /*
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
        */
    }
}
