using Server_Manager.Scripts;
using Server_Manager.Scripts.Initialization;
using ServerManagerFramework;
using ServerManagerFramework.Config;
using ServerManagerFramework.Installing;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Server_Manager.Views
{
    /// <summary>
    /// Interaction logic for AddServer.xaml
    /// </summary>
    public partial class AddServer : Grid, IHasTopMenuItems
    {
        public UIElement[] Items { get; } = new UIElement[2]
        {
            new ComboBox()
            {
                Width = 150,
                Margin = new Thickness(9),
                SelectedIndex = 0
            },
            new TextBlock()
            {
                Style = Application.Current.Resources["Header"] as Style,
                Foreground = SMR.WhiteBrush,
                Text = "Add Server"
            }
        };

        public AddServer()
        {
            InitializeComponent();

            ComboBox comboBox = Items[0] as ComboBox;

            comboBox.Style = Resources["ServerComboBox"] as Style;
            comboBox.ItemContainerStyle = Resources["ServerComboBoxItem"] as Style;
            comboBox.SelectionChanged += ComboBoxSelected;
            comboBox.ItemsSource = from installable
                                   in Initializer.Installables
                                   select installable.Key;
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            _ = MainWindow.GetMainWindow.ChangeCurrentControl(new ServerList());

            ComboBox comboBox = Items[0] as ComboBox;
            string key = comboBox.SelectedValue as string;
            Installable installable = Initializer.Installables[key];

            string path = Path.Combine(GlobalConfig.ServersPath, ServerName.Text);

            if (Directory.Exists(path))
            {
                return;
            }

            _ = Initializer.CreateServer(path, installable.ServerType, installable.InstallAction);
        }

        private void OnBackClick(object sender, RoutedEventArgs e)
        {
            _ = MainWindow.GetMainWindow.ChangeCurrentControl(new ServerList());
        }

        private void ComboBoxSelected(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            string key = comboBox.SelectedItem as string;

            items.Children.RemoveRange(2, items.Children.Count - 2);
            Array.ForEach(Initializer.Installables[key].Elements, (UIElement element) => items.Children.Add(element));
        }
    }
}
