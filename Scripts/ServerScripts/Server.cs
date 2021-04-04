using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Server_Manager.Scripts.ServerScripts
{
    public abstract class Server
    {
        #region Names and Paths
        public string Name { get; protected set; }
        public abstract string ParentDirectory { get; }
        public virtual string JarName { get; } = "server.jar";
        public string ServerDirectory { get => Path.Combine(ParentDirectory, Name); }
        string PropertiesPath { get => Path.Combine(ServerDirectory, "server.properties"); }
        string IconPath { get => Path.Combine(ServerDirectory, "server-icon.png"); }

        public void ChangeName(string name)
        {
            string newDir = Path.Combine(ParentDirectory, name);
            if (Directory.Exists(newDir)) return;

            Directory.Move(ServerDirectory, newDir);

            Name = name;

        }
        #endregion

        public readonly int arrayIndex;

        State state = State.stopped;
        public State State
        {
            get => state;
            protected set
            {
                state = value;
                Trace.WriteLine("Server is " + value.ToString());
                stateChange?.Invoke(this, EventArgs.Empty);
            }
        }
        public EventHandler stateChange;

        public Server(string name, int arrayIndex)
        {
            Name = name;
            this.arrayIndex = arrayIndex;

            StartArgs.ChangeJarName(JarName);
            StartArgs.AddArg("nogui");

            Application.Current.Exit += OnApplicationExit;
        }

        public abstract void Install();

        #region Process
        public Process Process { get; protected set; }
        int processID = -1;
        public readonly StartArgs StartArgs = new StartArgs();

        public virtual void Start()
        {
            if (State != State.stopped) return;

            Process = new Process
            {
                StartInfo = new ProcessStartInfo("javaw.exe", StartArgs.ToArg())
                {
                    WorkingDirectory = ServerDirectory,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                },
 
                EnableRaisingEvents = true
            };
            Process.Exited += delegate
            {
                Process.Dispose();
                Application.Current.Dispatcher.Invoke(() => State = State.stopped);
            };

            State = State.starting;
            Process.Start();

            processID = Process.Id;
            Process.BeginOutputReadLine();

            Process.OutputDataReceived += (object sender, DataReceivedEventArgs args) =>
            {
                string data = args.Data;
                if (string.IsNullOrEmpty(data)) return;

                Trace.WriteLine(data);
                if (data.Contains("] [Server thread/INFO]: Done (")) Application.Current.Dispatcher.Invoke(() => State = State.started);
            };
        }
        public virtual void Stop()
        {
            if (State != State.started) return;

            State = State.stopping;

            Process.StandardInput.WriteLine("stop");
        }

        void OnApplicationExit(object sender, EventArgs args)
        {
            if (processID == -1) return;

            try
            {
                Process proc = Process.GetProcessById(processID);
                proc.Kill();
            }
            catch { return; }
        }
        #endregion

        #region Properties
        public List<NameValuePair> properties = new List<NameValuePair>();

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
        #endregion

        #region Icon
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
        #endregion
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

    public class StartArgs
    {
        readonly List<string> args = new List<string>(new string[1] { "-jar" });

        public void ChangeJarName(string name)
        {
            if (args.Count > 1) args[1] = name;
            else args.Add(name);
        }

        public void AddArg(string arg) => args.Add(arg);

        public void RemoveArg(string arg)
        {
            int index = args.FindIndex(o => o == arg);
            if (index != -1) args.RemoveAt(index);
        }

        public string ToArg()
        {
            string argLine = "";

            foreach (var arg in args) argLine += (' ' + arg);

            return argLine;
        }
    }
}
