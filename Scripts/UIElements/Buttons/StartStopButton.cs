using Server_Manager.Scripts.ServerScripts;
using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Server_Manager.Scripts.UIElements.Buttons
{
    public class StartStopButton : ServerButton
    {
        TextBlock status;

        static readonly SolidColorBrush greenLoadingBackground = new SolidColorBrush(new Color() { R = 0, G = 107, B = 53, A = 255 });
        static readonly SolidColorBrush redLoadingBackground = new SolidColorBrush(new Color() { R = 188, G = 45, B = 0, A = 255 });

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
                    IsEnabled = false;

                    //Update Visuals
                    Background = greenLoadingBackground;
                    status.Text = "Starting";
                    break;
                case State.started:
                    IsEnabled = true;

                    //Update Visuals
                    Background = Colors.redBrush;
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
                    Background = Colors.greenBrush;
                    status.Text = "Start";
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
            switch (Server.State)
            {
                case State.started:
                    Server.Stop();
                    break;
                case State.stopped:
                    Server.Start();
                    break;
                default:
                    break;
            }
        }
    }
}
