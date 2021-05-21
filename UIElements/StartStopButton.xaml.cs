using ServerManagerFramework;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Server_Manager.UIElements
{
    /// <summary>
    /// Interaction logic for StartStopButton.xaml
    /// </summary>
    public partial class StartStopButton : Button
    {
        public static readonly DependencyProperty IServerProperty =
            DependencyProperty.Register(
                "IServer",
                typeof(IServer),
                typeof(StartStopButton));

        public IServer IServer
        {
            get
            {
                return (IServer)GetValue(IServerProperty);
            }
            set
            {
                SetValue(IServerProperty, value);
            }
        }

        private static readonly SolidColorBrush greenLoadingBackground = new(new Color() { R = 0, G = 107, B = 53, A = 255 });
        private static readonly SolidColorBrush redLoadingBackground = new(new Color() { R = 188, G = 45, B = 0, A = 255 });

        public StartStopButton() : base()
        {
            InitializeComponent();

            MouseEnter += OnMouseEnter;
            MouseLeave += OnMouseLeave;
            Click += SwitchState;

            Loaded += delegate
            {
                if (IServer != null)
                {
                    IServer.StateChanged += OnStateChanged;
                }

                OnStateChanged(this, new StateChangedEventArgs(State.stopped));
            };
        }

        private void OnStateChanged(object sender, StateChangedEventArgs e)
        {
            if (IServer == null)
            {
                return;
            }

            switch (IServer.State)
            {
                case State.starting:
                    IsEnabled = false;

                    //Update Visuals
                    Background = greenLoadingBackground;
                    status.Text = "Starting";
                    break;
                case State.started:
                    IsEnabled = true;

                    //Update Visuals
                    Background = App.redBrush;
                    status.Text = "Stop";
                    break;
                case State.stopping:
                    IsEnabled = false;

                    //Update Visuals
                    Background = redLoadingBackground;
                    status.Text = "Stopping";
                    break;
                case State.stopped:
                    IsEnabled = true;

                    //Update Visuals
                    Background = App.greenBrush;
                    status.Text = "Start";
                    break;
                default:
                    break;
            }
        }

        private void OnMouseEnter(object o, EventArgs e)
        {
            switch (IServer.State)
            {
                case State.started:
                    Background = App.redHoverBrush;
                    break;
                case State.stopped:
                    Background = App.greenHoverBrush;
                    break;
                default:
                    break;
            }
        }

        private void OnMouseLeave(object o, EventArgs e)
        {
            switch (IServer.State)
            {
                case State.started:
                    Background = App.redBrush;
                    break;
                case State.stopped:
                    Background = App.greenBrush;
                    break;
                default:
                    break;
            }
        }

        private void SwitchState(object o, EventArgs e)
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
        }
    }
}
