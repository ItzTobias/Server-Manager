using Server_Manager.Scripts;
using System;
using System.Windows.Controls;

namespace Server_Manager.Viewmodels
{
    /// <summary>
    /// Interaktionslogik für ServerInfo.xaml
    /// </summary>
    public partial class ServerInfo : UserControl
    {
        public Server server;

        public ServerInfo()
        {
            InitializeComponent();
        }

        public void OnActivate()
        {
            InfoName.Text = server.Name;
            StartStopButton.Server = server;
        }

        public void OnBackClick(object sender, EventArgs args) => MainWindow.GetMainWindow.OpenMenu();
    }
}
