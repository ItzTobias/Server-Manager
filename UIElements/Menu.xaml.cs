using Server_Manager.Scripts;
using Server_Manager.Scripts.Initialization;
using Server_Manager.Scripts.ServerScripts;
using Server_Manager.Scripts.UIElements.Buttons;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

            Initializer.Initialized += (object sender, EventArgs e) =>
                {
                    ServerTypesComboBox.SelectedIndex = 0;
                    ServerTypesComboBox.ItemsSource = Initializer.InitializeComboBox();
                };
        }

        public void OpenServerInfo(object sender, RoutedEventArgs args)
        {
            Server server = ((ServerButton_old)sender).Server;

            MainWindow.GetMainWindow.OpenInfo(server);
        }

        private void ServerTypesComboBox_Selected(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            NameTypePair serverNameType = comboBox.SelectedItem as NameTypePair;

            Type serverType = serverNameType.Type;

            if (serverType == null)
            {
                ServersUI.ItemsSource = Initializer.HasDirectoryList.GetServers();
            }
            else
            {
                ServersUI.ItemsSource = Initializer.HasDirectoryList.GetServers(serverType);
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

                    if (mousePosition.X < mainWindow.Left + halfWindowWidth)
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
