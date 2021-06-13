using Server_Manager.Scripts;
using Server_Manager.Scripts.Initialization;
using ServerManagerFramework;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Server_Manager.Views
{
    /// <summary>
    /// Interaction logic for ServerList.xaml
    /// </summary>
    public partial class ServerList : ScrollViewer, IHasTopMenuItems
    {
        public UIElement[] Items { get; } = new UIElement[1]
        {
            new ComboBox()
            {
                Width = 150,
                Margin = new Thickness(9),
                SelectedIndex = 0
            }//,
            //new Button()
            //{
            //    Width = 45
            //}
        };



        public ServerList()
        {
            InitializeComponent();

            ComboBox comboBox = Items[0] as ComboBox;

            comboBox.Style = Resources["ServerComboBox"] as Style;
            comboBox.ItemContainerStyle = Resources["ServerComboBoxItem"] as Style;
            comboBox.SelectionChanged += ComboBoxSelected;
            comboBox.ItemsSource = Initializer.ComboBoxItems;

            //Button button = Items[1] as Button;

            //button.Style = Resources["AddServerButton"] as Style;
            //button.Click += AddServer;

            ServersUI.ItemsSource = Initializer.HasDirectoryList.ServerProcesses;
        }

        public void AddServer(object sender, RoutedEventArgs args)
        {
            Guid g = Guid.NewGuid();
            string GuidString = Convert.ToBase64String(g.ToByteArray());
            GuidString = GuidString.Replace("=", "-");
            GuidString = GuidString.Replace("+", "_");

            string serverPath = Path.Combine(GlobalConfig.ServersPath, GuidString);
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
