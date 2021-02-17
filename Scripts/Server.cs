using Server_Manager.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Server_Manager.Scripts
{
    public abstract class Server
    {
        public string Name { get; protected set; }
        public abstract string ParentDirectory { get; }
        public string ServerDirectory { get => Path.Combine(ParentDirectory, Name); }
        string PropertiesPath { get => Path.Combine(ServerDirectory, "server.properties"); }
        string IconPath { get => Path.Combine(ServerDirectory, "server-icon.png"); }

        State state = State.stopped;
        public State State
        {
            get => state; 
            protected set
            {
                state = value;
                stateChange?.Invoke(this, EventArgs.Empty);
            }
        }
        public EventHandler stateChange;

        public List<NameValuePair> properties = new List<NameValuePair>();

        public BitmapImage Icon 
        {
            get
            {
                if (!File.Exists(IconPath)) return null;

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new Uri(IconPath);
                image.EndInit();

                return image;
            }
            set
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(value));

                using FileStream stream = new FileStream(IconPath, FileMode.Create);
                encoder.Save(stream);
            }
        }

        public Server(string name) => Name = name;

        public abstract void Install();

        public virtual async Task Start()
        {
            if (State != State.stopped) return;

            State = State.starting;

            await Task.Delay(2000);

            State = State.started;
        }
        public virtual async Task Stop()
        {
            if (State != State.started) return;

            State = State.stopping;

            await Task.Delay(2000);

            State = State.stopped;
        }
        public void UpdateProperties()
        {
            if (!File.Exists(PropertiesPath)) return;
            string[] lines = File.ReadAllLines(PropertiesPath);

            properties.Clear();
            foreach (var property in lines)
            {
                string[] nameProperty = property.Split('=');
                if (nameProperty.Length != 2) continue;
                properties.Add(new NameValuePair(nameProperty[0], nameProperty[1]));
            }
        }
        public void SaveProperties()
        {
            List<string> props = new List<string>();

            foreach (var property in properties) props.Add(property.GetNameUnformatted() + '=' + property.Value);

            File.WriteAllLines(PropertiesPath, props);

        }

        public void ChangeName(string name)
        {
            string newDir = Path.Combine(ParentDirectory, name);
            if (Directory.Exists(newDir)) return;

            Directory.Move(ServerDirectory, newDir);

            Name = name;

        }

        public void ChangeIcon(string path)
        {
            if (path == null)
            {
                File.Delete(IconPath);
                return;
            }
            else if (!File.Exists(path)) return;

            if (File.Exists(IconPath)) File.Delete(IconPath);

            File.Copy(path, IconPath);
        }
    }

    public class NameValuePair
    {
        string name;
        public string Name { 
            get
            {
                string formattedName = string.Copy(name);

                formattedName = formattedName.Replace('-', ' ');
                formattedName = formattedName.ToUpper();

                return formattedName;
            }
            set => name = value;
        }
        public string Value { get; set; }

        public NameValuePair(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string GetNameUnformatted() => name;
    }

    public enum State
    {
        starting,
        started,
        stopping,
        stopped
    }

    public class Vanilla : Server
    {
        public override string ParentDirectory { get => Path.Combine(Settings.Default.SERVERS_PATH, "Vanilla"); }

        public Vanilla(string name) : base(name) { }

        public override void Install()
        {
            throw new NotImplementedException();
        }
    }
}
