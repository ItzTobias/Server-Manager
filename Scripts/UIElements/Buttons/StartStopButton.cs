using Server_Manager.Scripts.ServerScripts;
using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Server_Manager.Scripts.UIElements.Buttons
{
    public class StartStopButton : ServerButton
    {
        private TextBlock status;
        private static readonly SolidColorBrush greenLoadingBackground = new(new Color() { R = 0, G = 107, B = 53, A = 255 });
        private static readonly SolidColorBrush redLoadingBackground = new(new Color() { R = 188, G = 45, B = 0, A = 255 });

        public StartStopButton() : base()
        {
            MouseEnter += OnMouseEnter;
            MouseLeave += OnMouseLeave;
            Click += SwitchState;
            Loaded += delegate
            {
                status = (TextBlock)Content;

                if (Server != null)
                {
                    Server.stateChange += OnStateChange;
                }

                OnStateChange(Server, EventArgs.Empty);
            };
        }

        private void OnStateChange(object sender, EventArgs args)
        {
            if (status == null || Server == null || Server != (Server)sender)
            {
                return;
            }

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
            switch (Server.State)
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
            switch (Server.State)
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
