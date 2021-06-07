using Server_Manager.Scripts;
using Server_Manager.Scripts.Initialization;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Server_Manager.UIElements
{
    /// <summary>
    /// Interaction logic for ServerList.xaml
    /// </summary>
    public partial class ServerList : ScrollViewer, IHasTopMenuItems
    {
        public UIElement[] Items { get; } = new UIElement[2];

        public ServerList()
        {
            InitializeComponent();

            ComboBox comboBox = new()
            {
                Style = Resources["ServerComboBox"] as Style,
                ItemContainerStyle = Resources["ServerComboBoxItem"] as Style,
                Width = 150,
                Margin = new Thickness(9),
                SelectedIndex = 0
            };

            comboBox.SelectionChanged += ComboBoxSelected;

            Items[0] = comboBox;


            Button button = new()
            {
                Width = 45,
                Style = Resources["AddServerButton"] as Style
            };

            button.Click += AddServer;

            Items[1] = button;

            comboBox.ItemsSource = Initializer.ComboBoxItems;
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

        private void ComboBoxSelected(object sender, RoutedEventArgs e)
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
    }
}
