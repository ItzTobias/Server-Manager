using Server_Manager.Scripts;
using Server_Manager.Scripts.Initialization;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Server_Manager.Views
{
    /// <summary>
    /// Interaction logic for ServerList.xaml
    /// </summary>
    public partial class ServerList : ScrollViewer, IHasTopMenuItems
    {
        public UIElement[] Items { get; } = new UIElement[3]
        {
            new ComboBox()
            {
                Width = 150,
                Margin = new Thickness(9),
                SelectedIndex = 0
            },
            new Button()
            {
                Width = 45
            },
            new Button()
            {
                Width = 45
            }
        };



        public ServerList()
        {
            InitializeComponent();

            ComboBox comboBox = Items[0] as ComboBox;

            comboBox.Style = Resources["ServerComboBox"] as Style;
            comboBox.ItemContainerStyle = Resources["ServerComboBoxItem"] as Style;
            comboBox.SelectionChanged += ComboBoxSelected;
            comboBox.ItemsSource = Initializer.ComboBoxItems;

            Button addServerButton = Items[1] as Button;

            addServerButton.Style = Resources["AddServerButton"] as Style;
            addServerButton.Click += AddServer;

            Button reloadButton = Items[2] as Button;

            reloadButton.Style = Resources["ReloadButton"] as Style;
            reloadButton.Click += Reload;

            ServersUI.ItemsSource = Initializer.HasDirectoryList.Servers;
        }

        public void AddServer(object sender, RoutedEventArgs args)
        {
            _ = MainWindow.GetMainWindow.ChangeCurrentControl(new AddServer());
        }

        public void Reload(object sender, RoutedEventArgs args)
        {
            _ = Initializer.InitializeServers();
        }

        private void ComboBoxSelected(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            NameTypePair serverNameType = comboBox.SelectedItem as NameTypePair;

            Type serverType = serverNameType.Type;

            Initializer.HasDirectoryList.Filter(serverType);
        }
    }
}
