using Server_Manager.Scripts.ServerScripts;
using System.Windows;
using System.Windows.Controls;

namespace Server_Manager.Scripts.UIElements.Buttons
{
    public class ServerButton_old : Button
    {
        public static readonly DependencyProperty ServerProperty =
            DependencyProperty.Register(
            "Server",
            typeof(Server),
            typeof(ServerButton_old));

        public Server Server
        {
            get => (Server)GetValue(ServerProperty);
            set => SetValue(ServerProperty, value);
        }

        public ServerButton_old() : base() { }
    }
}
