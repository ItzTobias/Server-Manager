using Server_Manager.Views;
using ServerManagerFramework.Servers;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Server_Manager.UIElements
{
    /// <summary>
    /// Interaction logic for ServerButton.xaml
    /// </summary>
    public partial class ServerButton : Canvas, INotifyPropertyChanged
    {
        public static readonly DependencyProperty IHasDirectoryProperty =
                DependencyProperty.Register(
                        nameof(IHasDirectory),
                        typeof(IHasDirectory),
                        typeof(ServerButton));

        public IHasDirectory IHasDirectory
        {
            get => (IHasDirectory)GetValue(IHasDirectoryProperty);
            set => SetValue(IHasDirectoryProperty, value);
        }

        private string serverName;

        public event PropertyChangedEventHandler PropertyChanged;

        public string ServerName
        {
            get => serverName;
            set
            {
                serverName = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(ServerName)));
            }
        }

        public ServerButton()
        {
            InitializeComponent();

            Loaded += delegate
            {
                ServerName = Path.GetFileName(IHasDirectory.Directory);

                if (IHasDirectory is IServer)
                {
                    StartStopButton.Visibility = Visibility.Visible;
                }

                if (IHasDirectory is InstallingServer installingServer)
                {
                    IsEnabled = false;
                    InstallComment.Visibility = Visibility.Visible;

                    TextChanged(this, installingServer.Text);
                    PercentageChanged(this, installingServer.Percentage);

                    installingServer.TextChanged += TextChanged;
                    installingServer.PercentageChanged += PercentageChanged;
                }
            };
        }

        private void TextChanged(object sender, string text)
        {
            InstallComment.Text = text;
        }

        private void PercentageChanged(object sender, double percentage)
        {
            if (percentage < 0)
            {
                InstallProgress.Visibility = Visibility.Collapsed;
                return;
            }

            if (InstallProgress.Visibility != Visibility.Visible)
            {
                InstallProgress.Visibility = Visibility.Visible;
            }

            InstallProgress.Value = percentage;
        }

        private void Click(object sender, RoutedEventArgs e)
        {
            _ = MainWindow.GetMainWindow.ChangeCurrentControl(new ServerInfo(IHasDirectory));
        }
    }
}
