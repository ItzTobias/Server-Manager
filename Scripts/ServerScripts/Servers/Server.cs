using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Timers;
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
        public string ServerDirectory => Path.Combine(ParentDirectory, Name);
        private string PropertiesPath => Path.Combine(ServerDirectory, "server.properties");
        private string IconPath => Path.Combine(ServerDirectory, "server-icon.png");

        public void ChangeName(string name)
        {
            string newDir = Path.Combine(ParentDirectory, name);
            if (Directory.Exists(newDir))
            {
                return;
            }

            Directory.Move(ServerDirectory, newDir);

            Name = name;

        }
        #endregion

        public readonly int arrayIndex;
        private State state = State.stopped;
        public State State
        {
            get => state;
            protected set
            {
                state = value;
                Trace.WriteLine("Server is " + value.ToString());
                stateChange?.Invoke(this, new StateChangeEventArgs(value));
            }
        }
        public EventHandler<StateChangeEventArgs> stateChange;

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

        private int processID = -1;
        public readonly StartArgs StartArgs = new();
        private bool hasDone = false;
        private string lastMessage;

        public virtual void Start()
        {
            if (State != State.stopped)
            {
                return;
            }

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
            Process.Exited += OnProcessExited;

            State = State.starting;
            Process.Start();

            hasDone = false;
            processID = Process.Id;
            Process.BeginOutputReadLine();

            Process.OutputDataReceived += OnOutputDataReceived;
        }
        public virtual void Stop()
        {
            if (State != State.started)
            {
                return;
            }

            State = State.stopping;

            Process.StandardInput.WriteLine("stop");
        }

        private void OnOutputDataReceived(object sender, DataReceivedEventArgs args)
        {
            Application.Current?.Dispatcher.Invoke(() => WriteToOutput(args.Data));
        }

        private void WriteToOutput(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return;
            }

            //Trace.WriteLine(data);

            if (data == lastMessage && ConsoleLine.Lines.Count > 0)
            {
                ConsoleLine.IncreaseDupeCount();
            }
            else if (!hasDone && data.Contains("] [Server thread/INFO]: Done ("))
            {
                hasDone = true;
                Application.Current.Dispatcher.Invoke(() => State = State.started);
                ConsoleLine.Add(data, MessageType.green);
            }
            else
            {
                ConsoleLine.Add(data);
            }

            lastMessage = data;
        }

        public void WriteLine(string data)
        {
            if (state != State.started)
            {
                return;
            }

            Process.StandardInput.WriteLine(data);
            ConsoleLine.Add(data, MessageType.highlighted);
        }

        private void OnProcessExited(object sender, EventArgs args)
        {
            Process.Dispose();
            Process = null;
            Application.Current?.Dispatcher.Invoke(() => State = State.stopped);
        }

        private void OnApplicationExit(object sender, EventArgs args)
        {
            if (processID == -1)
            {
                return;
            }

            ManagementObjectSearcher processSearcher = new("Select * From Win32_Process Where ParentProcessID=" + processID);
            ManagementObjectCollection processCollection = processSearcher.Get();

            foreach (ManagementObject mo in processCollection)
            {
                Process process = Process.GetProcessById(Convert.ToInt32(mo["ProcessID"]));

                if (process.ProcessName != "javaw")
                {
                    Trace.WriteLine(process.ProcessName);
                    continue;
                }

                process.Kill();
            }
        }
        #endregion

        #region Properties
        public List<NameValuePair> properties = new();

        public void UpdateProperties()
        {
            if (!File.Exists(PropertiesPath))
            {
                return;
            }

            string[] lines = File.ReadAllLines(PropertiesPath);

            properties.Clear();
            foreach (var property in lines)
            {
                string[] nameProperty = property.Split('=');
                if (nameProperty.Length != 2)
                {
                    continue;
                }

                properties.Add(new NameValuePair(nameProperty[0], nameProperty[1]));
            }
        }
        public void SaveProperties()
        {
            List<string> props = new();

            foreach (var property in properties)
            {
                props.Add(property.GetNameUnformatted() + '=' + property.Value);
            }

            File.WriteAllLines(PropertiesPath, props);

        }
        #endregion

        #region Icon
        public BitmapImage Icon
        {
            get
            {
                if (!File.Exists(IconPath))
                {
                    return null;
                }

                BitmapImage image = new();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new Uri(IconPath);
                image.EndInit();

                return image;
            }
            set
            {
                PngBitmapEncoder encoder = new();
                encoder.Frames.Add(BitmapFrame.Create(value));

                using FileStream stream = new(IconPath, FileMode.Create);
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
            else if (!File.Exists(path))
            {
                return;
            }

            if (File.Exists(IconPath))
            {
                File.Delete(IconPath);
            }

            File.Copy(path, IconPath);
        }
        #endregion

        #region Testing
        private readonly Random random = new();
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public void InitRandom(double interval, bool repeatMessage)
        {
            Timer timer = new() { Interval = interval };

            if (repeatMessage)
            {
                timer.Elapsed += (object sender, ElapsedEventArgs args) =>
                    Application.Current?.Dispatcher.Invoke(() => WriteToOutput("[HH:MM:SS] [Thread]: TestMessage"));
            }
            else
            {
                string[] strings = new string[99999];
                for (int i = 0; i < strings.Length; i++)
                {
                    strings[i] = "[HH:MM:SS] [Thread]: " + RandomString();
                }

                int index = 0;
                timer.Elapsed += (object sender, ElapsedEventArgs args) => Application.Current?.Dispatcher.Invoke(() =>
                {
                    if (index > 2000)
                    {
                        return;
                    }

                    WriteToOutput(strings[index]);
                    index++;
                });
            }

            timer.Start();
        }

        private string RandomString(int length = 10)
        {
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        #endregion
    }
}
