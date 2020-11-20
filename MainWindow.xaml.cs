using Server_Manager.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Server_Manager
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ServerType _currentServerType = ServerType.Vanilla;
        public ServerType CurrentServerType 
        { 
            get => _currentServerType; 
            private set
            {
                _currentServerType = value;
                ServersUI.ItemsSource = _currentServerType switch
                {
                    ServerType.Vanilla => vanillaServers,
                    ServerType.Forge => forgeServers,
                    ServerType.Fabric => fabricServers,
                    ServerType.Spigot => spigotServers,
                    ServerType.Bukkit => bukkitServers,
                    _ => throw new Exception("Invalid ServerType")
                };
            }
        }

        readonly ObservableCollection<string> vanillaServers = new ObservableCollection<string>();
        readonly ObservableCollection<string> forgeServers   = new ObservableCollection<string>();
        readonly ObservableCollection<string> fabricServers  = new ObservableCollection<string>();
        readonly ObservableCollection<string> spigotServers  = new ObservableCollection<string>();
        readonly ObservableCollection<string> bukkitServers  = new ObservableCollection<string>();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            OnClick_Vanilla(null, null);
        }

        #region TopMenu Tab Functions
        void OnClick_Vanilla(object sender, RoutedEventArgs e)
        {
            ResetOldTab();
            CurrentServerType = ServerType.Vanilla;

            //foreach (var item in collection)
            //{
            //
            //}
            
            Trace.WriteLine("Property: " + Settings.Default.SERVERS_PATH);

            //Set new Tab
            TopMenu_GreenBox_Vanilla.Visibility = Visibility.Visible;
            TopMenu_Button_Vanilla.IsEnabled = false;
        }
        void OnClick_Forge(object sender, RoutedEventArgs e)
        {
            ResetOldTab();
            CurrentServerType = ServerType.Forge;

            //Set new Tab
            TopMenu_GreenBox_Forge.Visibility = Visibility.Visible;
            TopMenu_Button_Forge.IsEnabled = false;
        }
        void OnClick_Fabric(object sender, RoutedEventArgs e)
        {
            ResetOldTab();
            CurrentServerType = ServerType.Fabric;

            //Set new Tab
            TopMenu_GreenBox_Fabric.Visibility = Visibility.Visible;
            TopMenu_Button_Fabric.IsEnabled = false;
        }
        void OnClick_Spigot(object sender, RoutedEventArgs e)
        {
            ResetOldTab();
            CurrentServerType = ServerType.Spigot;

            //Set new Tab
            TopMenu_GreenBox_Spigot.Visibility = Visibility.Visible;
            TopMenu_Button_Spigot.IsEnabled = false;
        }
        void OnClick_Bukkit(object sender, RoutedEventArgs e)
        {
            ResetOldTab();
            CurrentServerType = ServerType.Bukkit;

            //Set new Tab
            TopMenu_GreenBox_Bukkit.Visibility = Visibility.Visible;
            TopMenu_Button_Bukkit.IsEnabled = false;
        }

        void ResetOldTab()
        {
            switch (CurrentServerType)
            {
                case ServerType.Vanilla:
                    TopMenu_GreenBox_Vanilla.Visibility = Visibility.Hidden;
                    TopMenu_Button_Vanilla.IsEnabled = true;
                    break;
                case ServerType.Forge:
                    TopMenu_GreenBox_Forge.Visibility = Visibility.Hidden;
                    TopMenu_Button_Forge.IsEnabled = true;
                    break;
                case ServerType.Fabric:
                    TopMenu_GreenBox_Fabric.Visibility = Visibility.Hidden;
                    TopMenu_Button_Fabric.IsEnabled = true;
                    break;
                case ServerType.Spigot:
                    TopMenu_GreenBox_Spigot.Visibility = Visibility.Hidden;
                    TopMenu_Button_Spigot.IsEnabled = true;
                    break;
                case ServerType.Bukkit:
                    TopMenu_GreenBox_Bukkit.Visibility = Visibility.Hidden;
                    TopMenu_Button_Bukkit.IsEnabled = true;
                    break;
            }
        }
        #endregion
    }

    public class StartStopButton : Button
    {
        public bool Started { get; set; }
    }
}
