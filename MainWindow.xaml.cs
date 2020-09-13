using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Server_Manager
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Server> Servers { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
            Servers = new List<Server>();

            OnClick_Vanilla(null, null);
        }

        #region TopMenu Tab Functions
        ServerType oldType = ServerType.Vanilla;

        void OnClick_Vanilla(object sender, RoutedEventArgs e) => OnClickTopMenuTab(ServerType.Vanilla);
        void OnClick_Forge(object sender, RoutedEventArgs e) => OnClickTopMenuTab(ServerType.Forge);
        void OnClick_Fabric(object sender, RoutedEventArgs e) => OnClickTopMenuTab(ServerType.Fabric);
        void OnClick_Spigot(object sender, RoutedEventArgs e) => OnClickTopMenuTab(ServerType.Spigot);
        void OnClick_Bukkit(object sender, RoutedEventArgs e) => OnClickTopMenuTab(ServerType.Bukkit);

        void OnClickTopMenuTab(ServerType serverType)
        {
            Trace.WriteLine("test");

            switch (oldType)
            {
                case ServerType.Vanilla:
                    TopMenu_GreenBox_Vanilla.Visibility = Visibility.Hidden;
                    TopMenu_Button_Vanilla.IsEnabled = true;
                    break;
                case ServerType.Forge:
                    TopMenu_GreenBox_Forge.Visibility = Visibility.Hidden;
                    TopMenu_Button_Forge.IsEnabled = true;
                    break;
                case ServerType.Fabric:
                    TopMenu_GreenBox_Fabric.Visibility = Visibility.Hidden;
                    TopMenu_Button_Fabric.IsEnabled = true;
                    break;
                case ServerType.Spigot:
                    TopMenu_GreenBox_Spigot.Visibility = Visibility.Hidden;
                    TopMenu_Button_Spigot.IsEnabled = true;
                    break;
                case ServerType.Bukkit:
                    TopMenu_GreenBox_Bukkit.Visibility = Visibility.Hidden;
                    TopMenu_Button_Bukkit.IsEnabled = true;
                    break;
            }

            switch (serverType)
            {
                case ServerType.Vanilla:
                    TopMenu_GreenBox_Vanilla.Visibility = Visibility.Visible;
                    TopMenu_Button_Vanilla.IsEnabled = false;
                    string[] dirs = Directory.GetDirectories(@"C:\Users\itzto\Desktop\MC Test Server");
                    for (int i = 0; i < dirs.Length; i++)
                        Servers.Add(new Server()
                        {
                            Name = Path.GetDirectoryName(dirs[i])
                        });
                    break;
                case ServerType.Forge:
                    TopMenu_GreenBox_Forge.Visibility = Visibility.Visible;
                    TopMenu_Button_Forge.IsEnabled = false;
                    break;
                case ServerType.Fabric:
                    TopMenu_GreenBox_Fabric.Visibility = Visibility.Visible;
                    TopMenu_Button_Fabric.IsEnabled = false;
                    break;
                case ServerType.Spigot:
                    TopMenu_GreenBox_Spigot.Visibility = Visibility.Visible;
                    TopMenu_Button_Spigot.IsEnabled = false;
                    break;
                case ServerType.Bukkit:
                    TopMenu_GreenBox_Bukkit.Visibility = Visibility.Visible;
                    TopMenu_Button_Bukkit.IsEnabled = false;
                    break;
            }

            oldType = serverType;
        }
        #endregion
    }
}
