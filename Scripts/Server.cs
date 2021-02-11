using Server_Manager.Properties;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Server_Manager.Scripts
{
    public abstract class Server
    {
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
        public string Name { get; protected set; }
        public abstract string Directory { get; }

        public EventHandler stateChange;

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
        public override string Directory { get => Path.Combine(Settings.Default.SERVERS_PATH, "Vanilla", Name); }

        public Vanilla(string name) : base(name) { }

        public override void Install()
        {
            throw new NotImplementedException();
        }
    }
}
