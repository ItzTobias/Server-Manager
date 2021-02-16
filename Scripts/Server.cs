using Server_Manager.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Server_Manager.Scripts
{
    public abstract class Server
    {
        public string Name { get; protected set; }
        public abstract string Directory { get; }
        string PropertiesPath { get => Path.Combine(Directory, "server.properties"); }

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

            foreach (var property in properties) props.Add(property.Name + '=' + property.Value);

            File.WriteAllLines(PropertiesPath, props);

        }
    }

    public class NameValuePair
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public NameValuePair(string name, string value)
        {
            Name = name;
            Value = value;
        }
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
        public static string VanillaDirectory { get => Path.Combine(Settings.Default.SERVERS_PATH, "Vanilla"); }
        public override string Directory { get => Path.Combine(VanillaDirectory, Name); }

        public Vanilla(string name) : base(name) { }

        public override void Install()
        {
            throw new NotImplementedException();
        }
    }
}
