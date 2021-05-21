using ServerManagerFramework;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Server_Manager.UIElements
{
    /// <summary>
    /// Interaction logic for ServerButton.xaml
    /// </summary>
    public partial class ServerButton : Button, INotifyPropertyChanged
    {
        public static readonly DependencyProperty IHasDirectoryProperty =
                DependencyProperty.Register(
                        nameof(IHasDirectory),
                        typeof(IHasDirectory),
                        typeof(ServerButton));

        public IHasDirectory IHasDirectory
        {
            get
            {
                return (IHasDirectory)GetValue(IHasDirectoryProperty);
            }
            set
            {
                SetValue(IHasDirectoryProperty, value);
            }
        }

        private string serverName;

        public event PropertyChangedEventHandler PropertyChanged;

        public string ServerName
        {
            get
            {
                return serverName;
            }
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
            };
        }
    }
}
