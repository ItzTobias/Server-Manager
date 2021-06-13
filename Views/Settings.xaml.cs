using Server_Manager.Scripts;
using ServerManagerFramework;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Server_Manager.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Grid, IHasTopMenuItems
    {
        public UIElement[] Items { get; } = new UIElement[1]
        {
            new TextBlock()
            {
                Style = Application.Current.Resources["Header"] as Style,
                Foreground = SMR.WhiteBrush,
                Text = "Settings"
            }
        };

        public static string ServersPath
        {
            get => GlobalConfig.GetValue("serverspath");
            set
            {
                if (!Directory.Exists(value))
                {
                    return;
                }

                GlobalConfig.SetValue("serverspath", value);
            }
        }
        public static string BackupsPath
        {
            get => GlobalConfig.GetValue("backupspath");
            set
            {
                if (!Directory.Exists(value))
                {
                    return;
                }

                GlobalConfig.SetValue("backupspath", value);
            }
        }

        public Settings()
        {
            InitializeComponent();
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            GlobalConfig.Save();
        }

        private void OnBackClick(object sender, RoutedEventArgs e)
        {
            _ = MainWindow.GetMainWindow.ChangeCurrentControl(new ServerList());
        }
    }
}