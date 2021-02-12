using Server_Manager.Scripts;
using Server_Manager.Viewmodels;
using System.Windows;

namespace Server_Manager
{
    public partial class MainWindow : Window
    {
        public static MainWindow GetMainWindow { get; private set; }
        public readonly Menu menu = new Menu();
        public readonly ServerInfo info = new ServerInfo();

        public MainWindow()
        {
            InitializeComponent();

            GetMainWindow = this;
            DataContext = menu;
        }

        public void OpenMenu() => DataContext = menu;
        public void OpenInfo(Server server)
        {
            DataContext = info;
            info.server = server;
            info.OnActivate();
        }
    }
}
