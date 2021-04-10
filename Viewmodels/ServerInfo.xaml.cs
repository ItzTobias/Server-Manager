﻿using Microsoft.Win32;
using Server_Manager.Scripts.ServerScripts;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
        string lastLine = "";
        int repeatCount = 0;

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
        }

        public void OnDeactivate()
        {
            server.stateChange -= OnServerStart;
        }

        void OnServerStart(object sender, EventArgs e)
        {
            server.Process.OutputDataReceived += (object sender, DataReceivedEventArgs args) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    bool scrollToEnd = ConsoleScrollViewer.VerticalOffset == ConsoleScrollViewer.ScrollableHeight;

                    BlockCollection blocks = ConsoleTextBlock.Document.Blocks;
                    string value = args.Data;

                    FlowDocument flowDocument = new FlowDocument();
                    Paragraph paragraph = new Paragraph();
                    InlineCollection inlines = paragraph.Inlines;

                    if (blocks.Count == 0)
                    {
                        inlines.Add(value);
                        lastLine = value;
                    }
                    else if (value == lastLine)
                    {
                        repeatCount++;
                        if (repeatCount == 1) ((Paragraph)blocks.LastBlock).Inlines.Add("(2)");
                        //else this.value.Replace(repeatCount.ToString(), (repeatCount + 1).ToString(), this.value.Length - 2, 1);
                    }
                    else
                    {
                        inlines.Add('\n' + value);
                        repeatCount = 0;
                        lastLine = value;
                    }

                    flowDocument.Blocks.Add(paragraph);
                    ConsoleTextBlock.Document = flowDocument;

                    if (scrollToEnd) ConsoleScrollViewer.ScrollToEnd();
                });

            };
        }



        #region ButtonEvents
        void OnBackClick(object sender, EventArgs args) => MainWindow.GetMainWindow.OpenMenu();
        void OnSaveClick(object sender, EventArgs args)
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
        void SelectAllText(object sender, EventArgs args) => ((TextBox)sender).SelectAll();
        void OnChangeServerIconClick(object sender, EventArgs args)
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
        void OnDeleteIcon(object sender, EventArgs args)
        {
            server.ChangeIcon(null);

            UpdateIcon();
        }
        void OpenServerDirectory(object sender, EventArgs args) => Process.Start("explorer.exe", server.ServerDirectory);
        void DeleteServer(object sender, EventArgs args)
        {
            try 
            { 
                Menu.VanillaServers.RemoveAt(server.arrayIndex);
                Directory.Delete(server.ServerDirectory, true);
                MainWindow.GetMainWindow.OpenMenu();
            }
            catch  (Exception e) { Trace.WriteLine(e.Message); }
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
    }
}
