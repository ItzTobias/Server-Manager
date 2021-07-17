using Server_Manager.Scripts;
using ServerManagerFramework;
using ServerManagerFramework.Config;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Server_Manager.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Grid, IHasTopMenuItems, INotifyPropertyChanged
    {
        public UIElement[] Items { get; } = new UIElement[1]
        {
            new TextBlock()
            {
                Style = System.Windows.Application.Current.Resources["Header"] as Style,
                Foreground = SMR.WhiteBrush,
                Text = "Settings"
            }
        };

        public string ServersPath
        {
            get => GlobalConfig.ServersPath;
            set
            {
                if (!Directory.Exists(value))
                {
                    return;
                }

                GlobalConfig.SetValue("serverspath", value);

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ServersPath)));
            }
        }
        public string BackupsPath
        {
            get => GlobalConfig.BackupsPath;
            set
            {
                if (!Directory.Exists(value))
                {
                    return;
                }

                GlobalConfig.SetValue("backupspath", value);

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BackupsPath)));
            }
        }

        public Settings()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            GlobalConfig.Save();
        }

        private void OnBackClick(object sender, RoutedEventArgs e)
        {
            _ = MainWindow.GetMainWindow.ChangeCurrentControl(new ServerList());
        }

        private void PickServersFolder(object sender, RoutedEventArgs e)
        {
            using FolderBrowserDialog fileBrowser = new();
            DialogResult result = fileBrowser.ShowDialog();

            if (result == DialogResult.OK)
            {
                ServersPath = fileBrowser.SelectedPath;
            }
        }

        private void PickBackupsFolder(object sender, RoutedEventArgs e)
        {
            using FolderBrowserDialog fileBrowser = new();
            DialogResult result = fileBrowser.ShowDialog();

            if (result == DialogResult.OK)
            {
                BackupsPath = fileBrowser.SelectedPath;
            }
        }

        private void OpenServerManagerFolder(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", '"' + GlobalConfig.ManagerPath + '"');
        }
    }
}