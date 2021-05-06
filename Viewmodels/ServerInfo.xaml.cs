using Microsoft.Win32;
using Server_Manager.Scripts.ServerScripts;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Server_Manager.Viewmodels
{
    /// <summary>
    /// Interaktionslogik für ServerInfo.xaml
    /// </summary>
    public partial class ServerInfo : UserControl
    {
        public Server server;
        string lastCommand = "";

        public ServerInfo() => InitializeComponent();

        public void OnActivate()
        {
            StartStopButton.Server = server;

            //Load Name
            ServerName.Text = server.Name;

            //Load Icon
            UpdateIcon();

            //Load Properties
            server.UpdateProperties();
            ServerProperties.ItemsSource = server.properties;

            //Assign ItemsControl of Console
            CommandLine.PreviewKeyDown += CommandLineKeyDown;
            ConsoleItemsControl.ItemsSource = ConsoleLine.Lines;

            /*server.InitRandom(10, false); -----TESTING-----

            Timer timer = new Timer
            {
                Interval = 1000
            };
            timer.Elapsed += delegate
            {
                Trace.WriteLine(ConsoleLine.Lines.Count);
            };
            timer.Start();*/
        }

        void SendCommand()
        {
            lastCommand = CommandLine.Text;
            server.WriteLine(CommandLine.Text);
            CommandLine.Clear();
        }

        #region ButtonEvents
        void OnBackClick(object sender, EventArgs e) => MainWindow.GetMainWindow.OpenMenu();
        void OnSaveClick(object sender, EventArgs e)
        {
            //Save Name
            string newName = ServerName.Text;

            if (server.Name != newName)
                server.ChangeName(newName);

            //Save Properties
            for (int i = 0; i < ServerProperties.Items.Count; i++)
            {
                var container = ServerProperties.ItemContainerGenerator.ContainerFromIndex(i);

                if (container == null) continue;

                var nameValuePair = (NameValuePair)container.GetValue(ContentProperty);

                server.properties[i].Value = nameValuePair.Value;
            }

            server.SaveProperties();
        }
        void SelectAllText(object sender, EventArgs e) => ((TextBox)sender).SelectAll();
        void OnChangeServerIconClick(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "PNG-Images (.png)|*.png",
                DefaultExt = ".png",
                FileName = "server-icon"
            };

            bool? result = dialog.ShowDialog();

            if (result != true || dialog.FileName == string.Empty) return;

            server.ChangeIcon(dialog.FileName);
            UpdateIcon();
        }
        void OnDeleteIcon(object sender, EventArgs e)
        {
            server.ChangeIcon(null);

            UpdateIcon();
        }
        void OpenServerDirectory(object sender, EventArgs e) => Process.Start("explorer.exe", server.ServerDirectory);
        void DeleteServer(object sender, EventArgs e)
        {
            try 
            { 
                Menu.VanillaServers.RemoveAt(server.arrayIndex);
                Directory.Delete(server.ServerDirectory, true);
                MainWindow.GetMainWindow.OpenMenu();
            }
            catch  (Exception ex) { Trace.WriteLine(ex.Message); }
        }
        #endregion

        void UpdateIcon()
        {
            BitmapImage serverIcon = server.Icon;
            if (serverIcon == null)
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri("pack://application:,,,/Viewmodels/Images/ServerInfoButtonIcons/default_server.png");
                image.EndInit();
                ChangeServerIcon.Background = new ImageBrush(image);
            }
            else ChangeServerIcon.Background = new ImageBrush(serverIcon);
        }

        void ConsoleLineAdded(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ConsoleScrollViewer.VerticalOffset <= 1f) ConsoleScrollViewer.ScrollToEnd();
        }

        void ClearConsoleOutput(object sender, RoutedEventArgs e) => ConsoleLine.Lines.Clear();

        void ToggleTimeVisibilityOn(object sender, RoutedEventArgs e) => ConsoleLine.SetColumn(0, ColumnAction.Show);
        void ToggleThreadVisibilityOn(object sender, RoutedEventArgs e) => ConsoleLine.SetColumn(1, ColumnAction.Show);
        void ToggleTimeVisibilityOff(object sender, RoutedEventArgs e) => ConsoleLine.SetColumn(0, ColumnAction.Hide);
        void ToggleThreadVisibilityOff(object sender, RoutedEventArgs e) => ConsoleLine.SetColumn(1, ColumnAction.Hide);

        void SendCommand(object sender, RoutedEventArgs e) => SendCommand();
        private void CommandLineKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Return:
                    SendCommand();
                    break;
                case Key.Up:
                    Trace.WriteLine("ArrowUp");
                    CommandLine.Text = lastCommand;
                    break;
                default:
                    break;
            }
        }
    }
}
