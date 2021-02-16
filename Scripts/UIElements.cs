using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Server_Manager.Scripts
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
    public class StartStopButton : ServerButton
    {
        TextBlock status;

        static readonly SolidColorBrush greenLoadingBackground = new SolidColorBrush(new Color() { R = 0, G = 107, B = 53, A = 255 });
        static readonly SolidColorBrush redLoadingBackground   = new SolidColorBrush(new Color() { R = 188, G = 45, B = 0, A = 255 });

        public StartStopButton() : base()
        {
            MouseEnter += OnMouseEnter;
            MouseLeave += OnMouseLeave;
            Click += SwitchState;
            Loaded += delegate
            {
                status = (TextBlock)Content;

                if (Server != null) Server.stateChange += OnStateChange;
                OnStateChange(Server, EventArgs.Empty);
            };
        }

        void OnStateChange(object sender, EventArgs args)
        {
            if (status == null || Server == null || Server != (Server)sender) return;

            switch (Server.State)
            {
                case State.starting:
                    SetToLaunching();
                    break;
                case State.started:
                    SetToRunning();
                    break;
                case State.stopping:
                    SetToStopping();
                    break;
                case State.stopped:
                    SetToOff();
                    break;
                default:
                    break;
            }
        }
        
        void OnMouseEnter(object o, EventArgs e)
        {
            switch (Server.State)
            {
                case State.started:
                    Background = Colors.redHoverBrush;
                    break;
                case State.stopped:
                    Background = Colors.greenHoverBrush;
                    break;
                default:
                    break;
            }
        }
        void OnMouseLeave(object o, EventArgs e)
        {
            switch (Server.State)
            {
                case State.started:
                    Background = Colors.redBrush;
                    break;
                case State.stopped:
                    Background = Colors.greenBrush;
                    break;
                default:
                    break;
            }
        }

        void SwitchState(object o, EventArgs e)
        {
            IsEnabled = false;

            switch (Server.State)
            {
                case State.started:
                    Stop();
                    break;
                case State.stopped:
                    Start();
                    break;
                default:
                    break;
            }

            IsEnabled = true;
        }
        async void Start()
        {
            SetToLaunching();

            await Server.Start();

            SetToRunning();
        }
        async void Stop()
        {
            SetToStopping();

            await Server.Stop();

            SetToOff();
        }
        void SetToLaunching()
        {
            Background = greenLoadingBackground;
            status.Text = "Starting";
        }
        void SetToRunning()
        {
            Background = Colors.redBrush;
            status.Text = "Stop";
        }
        void SetToStopping()
        {
            Background = redLoadingBackground;
            status.Text = "Stopping";
        }
        void SetToOff()
        {
            Background = Colors.greenBrush;
            status.Text = "Start";
        }
    }
}