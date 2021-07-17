using Server_Manager.Scripts;
using Server_Manager.Scripts.Initialization;
using Server_Manager.UIElements;
using Server_Manager.Views;
using ServerManagerFramework;
using ServerManagerFramework.Config;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Server_Manager
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly TextBlock errorHeader = new()
        {
            Style = Application.Current.Resources["Header"] as Style,
            Foreground = SMR.RedBrush
        };
        private bool isDisplayingErrorMessage;
        private FrameworkElement oldControl;

        public event PropertyChangedEventHandler PropertyChanged;
        public FrameworkElement CurrentControl { get; private set; }

        public static MainWindow GetMainWindow { get; private set; }

        public MainWindow()
        {
            GlobalConfig.Load();

            InitializeComponent();

            StateChanged += WindowStateChanged;
            GetMainWindow = this;
            DockPanel.SetDock(errorHeader, Dock.Left);
            SettingsButton.Click += delegate
            {
                _ = ChangeCurrentControl(new Settings());
            };

            CurrentControl = new ServerList();

            IHasTopMenuItems topMenuItems = CurrentControl as IHasTopMenuItems;

            foreach (UIElement item in topMenuItems.Items)
            {
                item.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Left);
                TopMenuItemPanel.Children.Add(item);
                DockPanel.SetDock(item, Dock.Left);
            }

            Initializer.Initialize();
        }

        //Control actions
        public async Task ChangeCurrentControl(FrameworkElement newControl)
        {
            if (isDisplayingErrorMessage)
            {
                oldControl = newControl;

                return;
            }

            TimeSpan animationTime = new(0, 0, 0, 0, 100);

            for (int i = 4; i < TopMenuItemPanel.Children.Count; i++)
            {
                TopMenuItemPanel.Children[i].BeginAnimation(OpacityProperty, new DoubleAnimation(0, animationTime));
            }
            CurrentControl?.BeginAnimation(OpacityProperty, new DoubleAnimation(0, animationTime));

            await Task.Delay(animationTime);

            TopMenuItemPanel.Children.RemoveRange(4, TopMenuItemPanel.Children.Count - 1);
            CurrentControl = newControl;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentControl)));

            if (CurrentControl is not IHasTopMenuItems)
            {
                return;
            }

            IHasTopMenuItems topMenuItems = CurrentControl as IHasTopMenuItems;

            foreach (UIElement item in topMenuItems.Items)
            {
                item.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Left);
                TopMenuItemPanel.Children.Add(item);
                DockPanel.SetDock(item, Dock.Left);
                item.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, animationTime));
            }
            CurrentControl.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, animationTime));
        }
        public async Task ShowErrorMessage(ErrorMessage message)
        {
            if (isDisplayingErrorMessage)
            {
                return;
            }
            isDisplayingErrorMessage = true;

            TimeSpan animationTime = new(0, 0, 0, 0, 100);

            for (int i = 4; i < TopMenuItemPanel.Children.Count; i++)
            {
                TopMenuItemPanel.Children[i].BeginAnimation(OpacityProperty, new DoubleAnimation(0, animationTime));
            }
            CurrentControl?.BeginAnimation(OpacityProperty, new DoubleAnimation(0, animationTime));
            SettingsButton.BeginAnimation(OpacityProperty, new DoubleAnimation(0, animationTime));

            await Task.Delay(animationTime);

            oldControl = CurrentControl;
            CurrentControl = message;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentControl)));

            TopMenuItemPanel.Children.RemoveRange(4, TopMenuItemPanel.Children.Count - 1);

            errorHeader.Text = message.Text;

            TopMenuItemPanel.Children.Add(errorHeader);

            SettingsButton.Visibility = Visibility.Hidden;

            message.Initialize();

            errorHeader.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, animationTime));
            CurrentControl.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, animationTime));
        }
        public void HideErrorMessage()
        {
            if (!isDisplayingErrorMessage)
            {
                return;
            }
            isDisplayingErrorMessage = false;

            _ = ChangeCurrentControl(oldControl);
            oldControl = null;

            SettingsButton.Visibility = Visibility.Visible;

            SettingsButton.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, new TimeSpan(0, 0, 0, 0, 100)));
        }

        //Window actions
        private void WindowStateChanged(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    BorderThickness = new Thickness(0);
                    break;
                case WindowState.Maximized:
                    BorderThickness = new Thickness(5);
                    break;
                default:
                    break;
            }
        }
        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                Point mousePosition = e.GetPosition(this);
                double relativeXPosition = mousePosition.X / ActualWidth;

                double finalXPosition = 0;
                double screenWidth = 0;
                if (relativeXPosition < .5)
                {
                    finalXPosition = 0;
                }
                else
                {
                    screenWidth = ActualWidth;
                }

                MaximizeRestoreButton.ChangeState(this, new RoutedEventArgs(e.RoutedEvent, sender));

                double halfWindowWidth = ActualWidth / 2;
                if (relativeXPosition < .5)
                {
                    if (mousePosition.X > halfWindowWidth)
                    {
                        finalXPosition = mousePosition.X - halfWindowWidth;
                    }
                }
                else
                {
                    finalXPosition = screenWidth - ActualWidth;

                    if (mousePosition.X < halfWindowWidth + finalXPosition)
                    {
                        finalXPosition = mousePosition.X - halfWindowWidth;
                    }
                }

                Left = finalXPosition;
                Top = 0;
            }

            DragMove();
        }
        private void Close(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Minimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
