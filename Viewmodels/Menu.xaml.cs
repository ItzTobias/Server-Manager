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
        private ServerType selectedServerType = ServerType.Vanilla;
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

            ServerCollections.OnCollectionUpdate += OnCollectionUpdate;
            ServerCollections.UpdateAll();

            ServerTypesComboBox.ItemsSource = new ServerType[4] { ServerType.Vanilla, ServerType.Forge, ServerType.Fabric, ServerType.Bukkit };
            ServerTypesComboBox.SelectedIndex = 0;
        }

        private void OnCollectionUpdate(object sender, CollectionUpdateEventArgs args)
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

        private void ServerTypesComboBox_Selected(object sender, RoutedEventArgs e)
        {
            SelectedServerType = (ServerType)((ComboBox)sender).SelectedItem;
        }
    }
}
