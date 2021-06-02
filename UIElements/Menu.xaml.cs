using Server_Manager.Scripts;
using Server_Manager.Scripts.Initialization;
using ServerManagerFramework;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Server_Manager.UIElements
{
    /// <summary>
    /// Interaktionslogik für Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        public Menu()
        {
            InitializeComponent();

            Initializer.AddonsLoaded += (object sender, EventArgs e) =>
                {
                    ServerTypesComboBox.SelectedIndex = 0;
                    ServerTypesComboBox.ItemsSource = Initializer.InitializeComboBox();
                };

            ServersUI.ItemsSource = Initializer.HasDirectoryList.ServerProcesses;
        }

        public void AddServer(object sender, RoutedEventArgs args)
        {
            Guid g = Guid.NewGuid();
            string GuidString = Convert.ToBase64String(g.ToByteArray());
            GuidString = GuidString.Replace("=", "-");
            GuidString = GuidString.Replace("+", "_");

            string serverPath = Path.Combine(Initializer.ServersPath, GuidString);
            Directory.CreateDirectory(serverPath);

            _ = Initializer.InitializeServer(serverPath);
        }

        private void ServerTypesComboBox_Selected(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            NameTypePair serverNameType = comboBox.SelectedItem as NameTypePair;

            Type serverType = serverNameType.Type;

            if (serverType == null)
            {
                Initializer.HasDirectoryList.ResetFilter();
            }
            else
            {
                Initializer.HasDirectoryList.Filter(serverType);
            }
        }
        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            MainWindow mainWindow = MainWindow.GetMainWindow;
            if (mainWindow.WindowState == WindowState.Maximized)
            {
                Point mousePosition = e.GetPosition(this);
                double relativeXPosition = mousePosition.X / mainWindow.ActualWidth;

                double finalXPosition = 0;
                double screenWidth = 0;
                if (relativeXPosition < .5)
                {
                    finalXPosition = 0;
                }
                else
                {
                    screenWidth = mainWindow.ActualWidth;
                }

                MaximizeRestoreButton.ChangeState(this, new RoutedEventArgs(e.RoutedEvent, sender));

                double halfWindowWidth = mainWindow.ActualWidth / 2;
                if (relativeXPosition < .5)
                {
                    if (mousePosition.X > halfWindowWidth)
                    {
                        finalXPosition = mousePosition.X - halfWindowWidth;
                    }
                }
                else
                {
                    finalXPosition = screenWidth - mainWindow.ActualWidth;

                    if (mousePosition.X < halfWindowWidth + finalXPosition)
                    {
                        finalXPosition = mousePosition.X - halfWindowWidth;
                    }
                }

                mainWindow.Left = finalXPosition;
                mainWindow.Top = 0;
            }

            mainWindow.DragMove();
        }
        private void Close(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Minimize(object sender, RoutedEventArgs e)
        {
            MainWindow.GetMainWindow.Minimize();
        }
    }
}
