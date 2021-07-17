using Microsoft.Win32;
using Server_Manager.Scripts;
using Server_Manager.Scripts.Initialization;
using ServerManagerFramework;
using ServerManagerFramework.Config;
using ServerManagerFramework.ServerInfo;
using ServerManagerFramework.Servers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Server_Manager.Views
{
    /// <summary>
    /// Interaction logic for ServerInfo.xaml
    /// </summary>
    public partial class ServerInfo : Grid, IHasTopMenuItems
    {
        private readonly List<string> commandHistory = new();
        private int currentCommandIndex = 1;
        private bool typed;
        private bool typeAction;

        public IHasDirectory IHasDirectory { get; }

        public ServerInfo(IHasDirectory iHasDirectory)
        {
            InitializeComponent();

            ServerName.TextChanged += OnServerNameChanged;

            IHasDirectory = iHasDirectory;

            //TopMenuItems
            Button openFolderButton = Items[0] as Button;
            openFolderButton.Style = Application.Current.Resources["OpenFolderButton"] as Style;
            openFolderButton.Margin = new Thickness(5);
            openFolderButton.Click += delegate
            {
                Process.Start("explorer.exe", '"' + IHasDirectory.Directory + '"');
            };

            //ServerName TextBox
            ServerName.Text = Path.GetFileName(IHasDirectory.Directory);

            //Enable UI elements depending on server types.
            CheckTypes();

            //Icon
            HasIconAttribute iconAttribute = IHasDirectory.GetType().GetCustomAttribute<HasIconAttribute>(true);

            if (iconAttribute != null)
            {
                ServerIcon.Visibility = Visibility.Visible;
                ServerIcon.ToolTip = iconAttribute.ToolTip;

                iconPath = Path.Combine(IHasDirectory.Directory, iconAttribute.IconPath);

                ChangeServerIcon.SetImage(iconPath);
            }

            KeyBinding saveAll = new(new Command(SaveAll), Key.S, ModifierKeys.Control);

            InputBindings.Add(saveAll);
        }

        private void CheckTypes()
        {
            if (IHasDirectory is IServer server)
            {
                StartStopButton.IServer = server;
                StartStopButton.Visibility = Visibility.Visible;
            }

            if (IHasDirectory is ITerminalOutput output)
            {
                Terminal.Visibility = Visibility.Visible;
                TerminalOutput.Visibility = Visibility.Visible;
                TopbarButtons.Visibility = Visibility.Visible;
                ClearButton.Visibility = Visibility.Visible;
                SearchBox.Visibility = Visibility.Visible;

                TerminalItemsControl.ItemsSource = output.TerminalLines;

                if (output.TerminalLines is ObservableCollection<TerminalLine> observableCommandLine)
                {
                    observableCommandLine.CollectionChanged += TerminalLineAdded;
                }
            }

            if (IHasDirectory is ITerminalInput input)
            {
                Terminal.Visibility = Visibility.Visible;
                Input.Visibility = Visibility.Visible;
                TopbarButtons.Visibility = Visibility.Visible;

                TerminalInput.PreviewKeyDown += TerminalInput_PreviewKeyDown;
            }

            if (IHasDirectory is IHasLog)
            {
                OpenLogsFolder.Visibility = Visibility.Visible;
            }

            if (IHasDirectory is IHasServerInfoItems serverInfoItems)
            {
                IEnumerable<ServerInfoItem> items = serverInfoItems.InitializeItems();

                foreach (ServerInfoItem item in items)
                {
                    Expander expander = new()
                    {
                        Margin = new Thickness(0, 0, 0, 16),
                        Style = Application.Current.Resources["ServerInfoExpander"] as Style,
                        Header = item.Name,
                        Content = item.Element
                    };

                    ItemsContainer.Children.Insert(ItemsContainer.Children.Count - 4, expander);

                    if (item.Element is ISavableServerInfoItem savable)
                    {
                        savable.ID = Guid.NewGuid();

                        savables.Add(savable);

                        savable.CanSaveChanged += (object sender, CanSaveChangedEventArgs e) =>
                        {
                            ISavableServerInfoItem item = sender as ISavableServerInfoItem;

                            int savableIndex = savables.FindIndex(s => s.ID == item.ID);
                            savables[savableIndex] = item;

                            UpdateSaveRequired();
                        };
                    }
                }
            }

            if (IHasDirectory is IHasStartArgs startArgs)
            {
                StartArgs.Visibility = Visibility.Visible;
                StartArgsHeader.Visibility = Visibility.Visible;

                StartArgs.Text = startArgs.Arguments;
            }
        }

        private void TerminalInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    SendCommand(this, e);
                    break;
                case Key.Up:
                    if (commandHistory.Count <= currentCommandIndex)
                    {
                        break;
                    }

                    currentCommandIndex++;

                    typeAction = true;

                    TerminalInput.Text = commandHistory[^currentCommandIndex];
                    break;
                case Key.Down:
                    if (currentCommandIndex < 2)
                    {
                        break;
                    }

                    currentCommandIndex--;

                    typeAction = true;

                    TerminalInput.Text = commandHistory[^currentCommandIndex];
                    break;
                default:
                    break;
            }
        }

        #region TopMenuItems
        public UIElement[] Items { get; } = new UIElement[2]
        {
            new Button(),
            new TextBlock()
            {
                Style = Application.Current.Resources["Header"] as Style,
                Foreground = SMR.WhiteBrush,
                Text = "Edit Server"
            }
        };
        #endregion

        #region Name
        private bool serverNameSaveRequired;

        private void OnServerNameChanged(object sender, RoutedEventArgs e)
        {

            if (ServerName.Text == Path.GetFileName(IHasDirectory.Directory))
            {
                if (serverNameSaveRequired)
                {
                    serverNameSaveRequired = false;
                    UpdateSaveRequired();
                }
            }
            else
            {
                if (!serverNameSaveRequired)
                {
                    serverNameSaveRequired = true;
                    UpdateSaveRequired();
                }
            }
        }
        private void SaveName()
        {
            if (ServerName.Text == Path.GetFileName(IHasDirectory.Directory))
            {
                return;
            }

            string newDirectory = Path.Combine(GlobalConfig.ServersPath, ServerName.Text);

            if (Directory.Exists(newDirectory))
            {
                return;
            }

            Directory.Move(IHasDirectory.Directory, newDirectory);

            typeof(IHasDirectory)
                .GetProperty(nameof(IHasDirectory.Directory))
                .SetValue(IHasDirectory, newDirectory);

            serverNameSaveRequired = false;
        }
        #endregion

        #region Icon
        private readonly string iconPath;
        private void OnChangeServerIcon(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Filter = "All files (*.*)|*.*"
            };

            bool? result = dialog.ShowDialog();

            if (result != true || dialog.FileName == string.Empty)
            {
                return;
            }

            File.Copy(dialog.FileName, iconPath);
            ChangeServerIcon.SetImage(iconPath);
        }
        private void OnDeleteIcon(object sender, RoutedEventArgs e)
        {
            try
            {
                File.Delete(iconPath);

                ChangeServerIcon.SetImage(iconPath);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }
        #endregion

        #region Terminal
        private void Clear(object sender, RoutedEventArgs e)
        {
            ITerminalOutput output = IHasDirectory as ITerminalOutput;
            output.ClearLines();
        }
        private void TerminalLineAdded(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (TerminalScrollViewer.VerticalOffset > TerminalScrollViewer.ScrollableHeight - 10)
            {
                TerminalScrollViewer.ScrollToEnd();
            }
        }
        private void TerminalInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (typeAction)
            {
                typeAction = false;
                return;
            }

            currentCommandIndex = 1;

            if (!typed)
            {
                commandHistory.Add(TerminalInput.Text);
                typed = true;
            }

            commandHistory[^1] = TerminalInput.Text;
        }
        private void SendCommand(object sender, RoutedEventArgs e)
        {
            typed = false;

            ITerminalInput input = IHasDirectory as ITerminalInput;
            input.WriteLine(TerminalInput.Text);

            if (IHasDirectory is ITerminalOutput output)
            {
                if (string.IsNullOrEmpty(TerminalInput.Text))
                {
                    TerminalInput.Text = " ";
                }

                output.AddLine(new TerminalLine()
                {
                    Message = TerminalInput.Text,
                    FontStyle = StyleSimulations.BoldSimulation,
                    FontColor = SMR.SWhiteBrush
                });
            }

            TerminalInput.Text = "";
        }
        private void SearchBox_TextChanged(object sender, RoutedEventArgs e)
        {

            ITerminalOutput output = IHasDirectory as ITerminalOutput;
            IEnumerable<TerminalLine> commandLines = output.TerminalLines;
            string searchBoxText = SearchBox.Text;

            if (string.IsNullOrWhiteSpace(searchBoxText))
            {
                TerminalItemsControl.ItemsSource = commandLines;

                return;
            }

            IEnumerable<TerminalLine> filteredLines = commandLines.Where(l => l.Message.Contains(searchBoxText));

            TerminalItemsControl.ItemsSource = filteredLines;
        }
        #endregion

        #region Args
        private bool startArgsSaveRequired;

        private void OnStartArgsChanged(object sender, RoutedEventArgs e)
        {
            IHasStartArgs startArgs = IHasDirectory as IHasStartArgs;

            if (StartArgs.Text == startArgs.Arguments)
            {
                if (startArgsSaveRequired)
                {
                    startArgsSaveRequired = false;
                    UpdateSaveRequired();
                }
            }
            else
            {
                if (!startArgsSaveRequired)
                {
                    startArgsSaveRequired = true;
                    UpdateSaveRequired();
                }
            }
        }
        private void SaveArgs()
        {
            IHasStartArgs startArgs = IHasDirectory as IHasStartArgs;

            if (StartArgs.Text == startArgs.Arguments)
            {
                return;
            }

            startArgs.Arguments = StartArgs.Text;

            startArgsSaveRequired = false;
        }
        #endregion

        #region Logs
        private void OnOpenLogsFolder(object sender, RoutedEventArgs e)
        {
            IHasLog log = IHasDirectory as IHasLog;

            string logsFolderPath = Path.Combine(IHasDirectory.Directory, log.LogFolder);

            Process.Start("explorer.exe", logsFolderPath);
        }
        #endregion

        #region Server Deletion
        private void OnDeleteServer(object sender, RoutedEventArgs e)
        {
            _ = MainWindow.GetMainWindow.ShowErrorMessage(new ErrorMessage()
            {
                Text = "Confirm server deletion",
                GrayButton = new Tuple<string, RoutedEventHandler>("Cancel", (object sender, RoutedEventArgs e) => MainWindow.GetMainWindow.HideErrorMessage()),
                RedButton = new Tuple<string, RoutedEventHandler>("Delete", DeleteServer)
            });
        }
        private void DeleteServer(object sender, RoutedEventArgs e)
        {
            Initializer.HasDirectoryList.RemoveServer(IHasDirectory);
            _ = MainWindow.GetMainWindow.ChangeCurrentControl(new ServerList());

            try
            {
                Directory.Delete(IHasDirectory.Directory, true);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }

            MainWindow.GetMainWindow.HideErrorMessage();
        }
        #endregion

        #region Saving
        private readonly List<ISavableServerInfoItem> savables = new();
        private void UpdateSaveRequired()
        {
            foreach (ISavableServerInfoItem savable in savables)
            {
                if (savable.CanSave)
                {
                    SetSaveRequired(true);
                    return;
                }
            }

            if (serverNameSaveRequired)
            {
                SetSaveRequired(true);
                return;
            }

            if (startArgsSaveRequired)
            {
                SetSaveRequired(true);
                return;
            }

            SetSaveRequired(false);
        }

        private void SaveAll()
        {
            if (!saveRequired)
            {
                return;
            }

            if (serverNameSaveRequired)
            {
                SaveName();
            }

            if (startArgsSaveRequired)
            {
                SaveArgs();
            }

            foreach (ISavableServerInfoItem savable in savables)
            {
                if (savable.CanSave)
                {
                    savable.Save();
                    savable.CanSave = false;
                }
            }

            string path = Path.Combine(IHasDirectory.Directory, Initializer.CONFIGFILENAME);

            _ = File.WriteAllTextAsync(path, IHasDirectory.Config.ToString());

            SetSaveRequired(false);
        }

        private bool saveRequired;
        private void SetSaveRequired(bool value)
        {
            TextBlock textBlock = Items[1] as TextBlock;

            textBlock.Text = value ? "Edit Server*" : "Edit Server";

            saveRequired = value;
        }
        #endregion

        #region Bottom buttons
        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            SaveAll();
        }
        private void OnBackClick(object sender, RoutedEventArgs e)
        {
            _ = MainWindow.GetMainWindow.ChangeCurrentControl(new ServerList());
        }
        #endregion

        private class Command : ICommand
        {
            private readonly Action action;

            public Command(Action action)
            {
                this.action = action;
            }

#pragma warning disable 0067
            public event EventHandler CanExecuteChanged;
#pragma warning restore

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                action();
            }
        }
    }
}