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

        public ServerInfo() => InitializeComponent();

        public void OnActivate()
        {
            InfoName.Text = server.Name;
            StartStopButton.Server = server;

            server.UpdateProperties();
            ServerProperties.ItemsSource = server.properties;
        }

        void OnBackClick(object sender, EventArgs args) => MainWindow.GetMainWindow.OpenMenu();
        void OnSaveClick(object sender, EventArgs args)
        {
            for (int i = 0; i < ServerProperties.Items.Count; i++)
            {
                var container = ServerProperties.ItemContainerGenerator.ContainerFromIndex(i);
                var nameValuePair = (NameValuePair)container.GetValue(ContentProperty);

                server.properties[i].Value = nameValuePair.Value;
            }

            server.SaveProperties();
        }

        private void SelectAllText(object sender, EventArgs args) => ((TextBox)sender).SelectAll();
    }
}
