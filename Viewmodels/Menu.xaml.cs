using Server_Manager.Scripts;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Server_Manager.Viewmodels
{
    /// <summary>
    /// Interaktionslogik für Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
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

        readonly ObservableCollection<Vanilla> vanillaServers = new ObservableCollection<Vanilla>();
        readonly ObservableCollection<Server> forgeServers = new ObservableCollection<Server>();
        readonly ObservableCollection<Server> fabricServers = new ObservableCollection<Server>();
        readonly ObservableCollection<Server> spigotServers = new ObservableCollection<Server>();
        readonly ObservableCollection<Server> bukkitServers = new ObservableCollection<Server>();

        public Menu()
        {
            InitializeComponent();

            DataContext = this;

            OnClick_Vanilla(this, null);

            InitializeServers();
        }

        public void OpenServerInfo(object sender, RoutedEventArgs args)
        {
            Server server = ((ServerButton)sender).Server;

            MainWindow.GetMainWindow.OpenInfo(server);
        }

        #region TopMenu Tab Functions
        void OnClick_Vanilla(object sender, RoutedEventArgs e) => ChangeTab(ServerType.Vanilla);
        void OnClick_Forge(object sender, RoutedEventArgs e) => ChangeTab(ServerType.Forge);
        void OnClick_Fabric(object sender, RoutedEventArgs e) => ChangeTab(ServerType.Fabric);
        void OnClick_Spigot(object sender, RoutedEventArgs e) => ChangeTab(ServerType.Spigot);
        void OnClick_Bukkit(object sender, RoutedEventArgs e) => ChangeTab(ServerType.Bukkit);

        void ChangeTab(ServerType newServerType)
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

            CurrentServerType = newServerType;

            switch (CurrentServerType)
            {
                case ServerType.Vanilla:
                    TopMenu_GreenBox_Vanilla.Visibility = Visibility.Visible;
                    TopMenu_Button_Vanilla.IsEnabled = false;
                    break;
                case ServerType.Forge:
                    TopMenu_GreenBox_Forge.Visibility = Visibility.Visible;
                    TopMenu_Button_Forge.IsEnabled = false;
                    break;
                case ServerType.Fabric:
                    TopMenu_GreenBox_Fabric.Visibility = Visibility.Visible;
                    TopMenu_Button_Fabric.IsEnabled = false;
                    break;
                case ServerType.Spigot:
                    TopMenu_GreenBox_Spigot.Visibility = Visibility.Visible;
                    TopMenu_Button_Spigot.IsEnabled = false;
                    break;
                case ServerType.Bukkit:
                    TopMenu_GreenBox_Bukkit.Visibility = Visibility.Visible;
                    TopMenu_Button_Bukkit.IsEnabled = false;
                    break;
            }
        }
        #endregion

        #region ServerInitializing
        void InitializeServers()
        {
            InitializeVanillaServers();
            InitializeForgeServers();
            InitializeFabricServers();
            InitializeSpigotServers();
            InitializeBukkitServers();
        }

        void InitializeVanillaServers()
        {
            string[] vanillaServers = Directory.GetDirectories(Vanilla.VanillaDirectory);

            Trace.WriteLine("Registered " + vanillaServers.Length + " Servers");

            foreach (string serverDir in vanillaServers)
                this.vanillaServers.Add(new Vanilla(Path.GetFileName(serverDir)));
        }
        void InitializeForgeServers()
        {
        }
        void InitializeFabricServers()
        {
        }
        void InitializeSpigotServers()
        {
        }
        void InitializeBukkitServers()
        {
        }
        #endregion
    }
}
