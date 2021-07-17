using ServerManagerFramework.Config;
using ServerManagerFramework.Servers;
using System;

namespace Server_Manager
{
    public class InstallingServer : IHasDirectory
    {
        public string Directory { get; init; }
        public Config Config { get; init; }
        public IHasDirectory NewServer { get; init; }

        private double percentage;
        public double Percentage
        {
            get => percentage;
            set
            {
                if (value == percentage)
                {
                    return;
                }

                percentage = value;
                PercentageChanged?.Invoke(this, value);
            }
        }

        private string text = "Installing server";
        public string Text
        {
            get => text;
            set
            {
                if (value == null || value == text)
                {
                    return;
                }

                text = value;
                TextChanged?.Invoke(this, value);
            }
        }

        public void Initialized()
        {

        }

        public void InvokeInstalled()
        {
            Installed.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Installed;
        public event EventHandler<double> PercentageChanged;
        public event EventHandler<string> TextChanged;
    }
}
