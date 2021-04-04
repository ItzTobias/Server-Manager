using Server_Manager.Scripts.ServerScripts;
using System.Windows;
using System.Windows.Controls;

namespace Server_Manager.Scripts.UIElements.Buttons
{
    public class ServerButton : Button
    {
        public static readonly DependencyProperty ServerProperty =
            DependencyProperty.Register(
            "Server",
            typeof(Server),
            typeof(ServerButton));

        public Server Server
        {
            get { return (Server)GetValue(ServerProperty); }
            set { SetValue(ServerProperty, value); }
        }

        public ServerButton() : base() { }
    }
}
