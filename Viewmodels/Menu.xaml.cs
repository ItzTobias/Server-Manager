using Server_Manager.Scripts.ServerScripts;
using Server_Manager.Scripts.UIElements.Buttons;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Server_Manager.Viewmodels
{
    /// <summary>
    /// Interaktionslogik für Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        ServerType selectedServerType = ServerType.Vanilla;
        public ServerType SelectedServerType
        {
            get => selectedServerType;
            private set
            {
                selectedServerType = value;
                ServersUI.ItemsSource = selectedServerType switch
                {
                    ServerType.Vanilla => VanillaServers,
                    ServerType.Forge => ForgeServers,
                    ServerType.Fabric => FabricServers,
                    ServerType.Bukkit => BukkitServers,
                    _ => throw new Exception("Invalid ServerType")
                };
            }
        }

        public static ObservableCollection<Vanilla> VanillaServers { get; private set; } = new ObservableCollection<Vanilla>();
        public static ObservableCollection<Server> ForgeServers { get; private set; } = new ObservableCollection<Server>();
        public static ObservableCollection<Server> FabricServers { get; private set; } = new ObservableCollection<Server>();
        public static ObservableCollection<Server> BukkitServers { get; private set; } = new ObservableCollection<Server>();

        public Menu()
        {
            InitializeComponent();

            DataContext = this;

            ServerCollections.onCollectionUpdate += OnCollectionUpdate;
            ServerCollections.UpdateAll();

            OnClick_Vanilla(this, null);
        }

        void OnCollectionUpdate(object sender, CollectionUpdateEventArgs args)
        {
            switch (args.collectionType)
            {
                case CollectionType.Vanilla:
                    VanillaServers = new ObservableCollection<Vanilla>(ServerCollections.Vanilla);
                    break;
                case CollectionType.Forge:
                    ForgeServers = new ObservableCollection<Server>(ServerCollections.Forge);
                    break;
                case CollectionType.Fabric:
                    FabricServers = new ObservableCollection<Server>(ServerCollections.Fabric);
                    break;
                case CollectionType.Bukkit:
                    BukkitServers = new ObservableCollection<Server>(ServerCollections.Bukkit);
                    break;
                default:
                    Trace.WriteLine("No Server Type is matching");
                    break;
            }
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
        void OnClick_Bukkit(object sender, RoutedEventArgs e) => ChangeTab(ServerType.Bukkit);

        void ChangeTab(ServerType newServerType)
        {
            switch (SelectedServerType)
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
                case ServerType.Bukkit:
                    TopMenu_GreenBox_Bukkit.Visibility = Visibility.Hidden;
                    TopMenu_Button_Bukkit.IsEnabled = true;
                    break;
            }

            SelectedServerType = newServerType;

            switch (SelectedServerType)
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
                case ServerType.Bukkit:
                    TopMenu_GreenBox_Bukkit.Visibility = Visibility.Visible;
                    TopMenu_Button_Bukkit.IsEnabled = false;
                    break;
            }
        }
        #endregion
    }
}
