using Server_Manager.Scripts;
using Server_Manager.Scripts.Initialization;
using Server_Manager.Scripts.ServerScripts;
using Server_Manager.Scripts.UIElements.Buttons;
using System;
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

            ServerTypesComboBox.ItemsSource = Initializer.InitializeComboBox();
            ServerTypesComboBox.SelectedIndex = 0;
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
            MainWindow.GetMainWindow.DragMove();
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
