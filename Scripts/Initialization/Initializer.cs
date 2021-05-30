using Server_Manager.Properties;
using ServerManagerFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Server_Manager.Scripts.Initialization
{
    public static class Initializer
    {
        public static event EventHandler Initialized;
        public static HasDirectoryList HasDirectoryList { get; } = new HasDirectoryList();
        public static void Initialize()
        {
            LoadAllAddons();

            List<Tuple<string, Config>> serverConfigs = InitializeConfigFiles();

            InitializeServers(serverConfigs);
        }

        #region Addons
        private static string AddonsPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + ADDONSPATH;
        private const string ADDONSPATH = @"\Server-Manager\Addons";

        private static readonly List<Assembly> addons = new();

        private static void LoadAllAddons()
        {
            Trace.WriteLine($"AddonsLoader: Loading addons.");

            List<string> assemblyLocations = new();

            string[] addonPaths = Directory.GetFiles(AddonsPath);

            foreach (string addonPath in addonPaths)
            {
                Assembly addonAssembly = Assembly.LoadFrom(addonPath);

                if (assemblyLocations.Contains(addonAssembly.Location))
                {
                    continue;
                }

                assemblyLocations.Add(addonAssembly.Location);


                addons.Add(addonAssembly);
            }

            Trace.WriteLine("AddonsLoader: Loaded " + addons.Count + " addons.");
        }
        #endregion

        #region Config
        private const string CONFIGFILENAMES = "server-manager.prefs";
        private static string DefaultConfig => "type=" + nameof(HasDirectory);

        private static List<Tuple<string, Config>> InitializeConfigFiles()
        {
            Trace.WriteLine($"ConfigLoader: Loading server configs.");

            FileInfo[] configFiles = GetConfigFiles();
            List<Tuple<string, Config>> serverConfigs = new();

            foreach (FileInfo configFileInfo in configFiles)
            {
                if (!configFileInfo.Exists)
                {
                    InitializeConfigFile(configFileInfo.FullName);
                }

                string path = configFileInfo.FullName;
                Config config = LoadConfig(configFileInfo);

                serverConfigs.Add(new Tuple<string, Config>(path, config));
            }

            Trace.WriteLine($"ConfigLoader: Loaded {serverConfigs.Count} server configs.");

            return serverConfigs;
        }

        private static FileInfo[] GetConfigFiles()
        {
            List<FileInfo> configFileInfos = new();

            string[] serverDirectories = Directory.GetDirectories(Settings.Default.SERVERS_PATH);
            foreach (string directory in serverDirectories)
            {
                string configFilePath = Path.Combine(directory, CONFIGFILENAMES);
                configFileInfos.Add(new FileInfo(configFilePath));
            }

            return configFileInfos.ToArray();
        }

        private static void InitializeConfigFile(string path)
        {
            File.WriteAllText(path, DefaultConfig);
        }

        private static Config LoadConfig(FileInfo configFileInfo)
        {
            string filePath = configFileInfo.FullName;

            string text = File.ReadAllText(filePath);

            return new Config(text);
        }
        #endregion

        #region Server Initialization
        private static void InitializeServers(List<Tuple<string, Config>> serverConfigs)
        {
            Trace.WriteLine($"ServerInitializer: Initializing servers.");

            foreach (Tuple<string, Config> config in serverConfigs)
            {
                string typeName = config.Item2.FindValue("type");

            noTypeName:
                if (typeName == null)
                {
                    InitializeConfigFile(config.Item1);
                    typeName = nameof(HasDirectory);
                }

                string directoryPath = Path.GetDirectoryName(config.Item1);

                if (typeName == nameof(HasDirectory))
                {
                    HasDirectoryList.AddServer(new HasDirectory(directoryPath));
                    continue;
                }

                bool typeFound = false;
                foreach (Assembly addon in addons)
                {
                    Type serverType = addon.GetType(typeName);

                    if (serverType == null)
                    {
                        continue;
                    }
                    else
                    {
                        IHasDirectory hasDirectory = Activator.CreateInstance(serverType, directoryPath) as IHasDirectory;
                        HasDirectoryList.AddServer(hasDirectory);
                        typeFound = true;
                        break;
                    }
                }

                if (!typeFound)
                {
                    typeName = null;
                    goto noTypeName;
                }
            }

            Initialized?.Invoke(typeof(Initializer), EventArgs.Empty);
        }

        public static List<NameTypePair> InitializeComboBox()
        {
            Trace.WriteLine($"ComboBoxInitializer: Initializing combobox.");

            List<NameTypePair> comboBoxTypes = new();

            comboBoxTypes.Add(new NameTypePair("All", null));
            comboBoxTypes.Add(new NameTypePair("Unknown", typeof(HasDirectory)));

            foreach (Assembly addon in addons)
            {
                foreach (Type serverType in addon.GetTypes())
                {
                    ComboBoxButtonAttribute comboBoxButtonAttribute = serverType.GetCustomAttribute<ComboBoxButtonAttribute>(false);

                    if (comboBoxButtonAttribute == null)
                    {
                        continue;
                    }
                    else
                    {
                        if (comboBoxButtonAttribute.useDefault)
                        {
                            comboBoxTypes.Add(new NameTypePair(serverType.Name, serverType));
                        }
                        else
                        {
                            comboBoxTypes.Add(new NameTypePair(comboBoxButtonAttribute.className, serverType));
                        }
                    }
                }
            }

            Trace.WriteLine($"ComboBoxInitializer: Initialized {comboBoxTypes.Count} buttons.");
            return comboBoxTypes;
        }
        #endregion
    }
}
