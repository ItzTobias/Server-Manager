using ServerManagerFramework;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Server_Manager.UIElements
{
    /// <summary>
    /// Interaction logic for StartStopButton.xaml
    /// </summary>
    public partial class StartStopButton : Button, INotifyPropertyChanged
    {
        public static readonly DependencyProperty IServerProperty =
            DependencyProperty.Register(
                "IServer",
                typeof(IServer),
                typeof(StartStopButton));

        public IServer IServer
        {
            get => (IServer)GetValue(IServerProperty);
            set => SetValue(IServerProperty, value);
        }

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(
                "State",
                typeof(State),
                typeof(StartStopButton),
                new PropertyMetadata(State.stopped));

        public event PropertyChangedEventHandler PropertyChanged;

        public State State
        {
            get => (State)GetValue(StateProperty);
            set
            {
                if (State == value)
                {
                    return;
                }

                SetValue(StateProperty, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
            }
        }

        #region Animation resources
        private readonly TimeSpan shortDuration = new(0, 0, 0, 0, 100);
        private readonly TimeSpan longDuration = new(0, 0, 0, 0, 500);
        #endregion

        public StartStopButton() : base()
        {
            InitializeComponent();

            Click += delegate
            {
                switch (IServer.State)
                {
                    case State.started:
                        IServer.Stop();
                        break;
                    case State.stopped:
                        IServer.Start();
                        break;
                    default:
                        break;
                }
            };

            Loaded += delegate
            {
                if (IServer != null)
                {
                    IServer.StateChanged += (object sender, StateChangedEventArgs e) => StartStopButton_StateChanged(sender, e);

                    SetValue(StateProperty, State.stopped);
                }
            };

            MouseEnter += StartStopButton_MouseEnter;
            MouseLeave += StartStopButton_MouseLeave;

            Foreground = App.FontColorBrush;
            Background = App.GreenBrush;
            Overlay.Background = App.TransparentBrush;
        }

        private void StartStopButton_MouseEnter(object sender, MouseEventArgs e)
        {
            Foreground.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(App.White, shortDuration));
            Overlay.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(App.Hover, shortDuration));
        }
        private void StartStopButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Foreground.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(App.FontColor, shortDuration));
            Overlay.Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(App.Transparent, shortDuration));
        }

        private void StartStopButton_StateChanged(object sender, StateChangedEventArgs e)
        {
            State = e.ServerState;

            switch (State)
            {
                case State.starting:
                    IsEnabled = false;
                    Status.Text = "Starting";
                    Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(App.Starting, longDuration));
                    break;
                case State.started:
                    IsEnabled = true;
                    Status.Text = "Stop";
                    Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(App.Red, longDuration));
                    break;
                case State.stopping:
                    IsEnabled = false;
                    Status.Text = "Stopping";
                    Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(App.Stopping, longDuration));
                    break;
                case State.stopped:
                    IsEnabled = true;
                    Status.Text = "Start";
                    Background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(App.Green, longDuration));
                    break;
            }
        }
    }
}