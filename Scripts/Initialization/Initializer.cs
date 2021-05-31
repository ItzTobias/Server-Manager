using Server_Manager.Properties;
using ServerManagerFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Server_Manager.Scripts.Initialization
{
    public static class Initializer
    {
        #region Path Variables
        private const string MANAGERPATH = @"\Server-Manager";
        public static string ManagerPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + MANAGERPATH;

        private const string SERVERSPATH = @"\Servers";
        public static string ServersPath => ManagerPath + SERVERSPATH;
        private const string ADDONSPATH = @"\Addons";
        public static string AddonsPath => ManagerPath + ADDONSPATH;
        #endregion

        private const string FRAMEWORKFILENAME = "ServerManagerFramework";

        private static readonly List<Assembly> addons = new();

        private const string CONFIGFILENAMES = "server-manager.prefs";
        private static string DefaultConfig => "type=" + nameof(HasDirectory);

        public static HasDirectoryList HasDirectoryList { get; } = new HasDirectoryList();

        public static event EventHandler Initialized;

        public static async void Initialize()
        {
            FindOrCreateFoldersAndFiles();

            LoadAllAddons();

            List<Tuple<string, Config>> serverConfigs = await InitializeConfigFiles();

            InitializeServers(serverConfigs);
        }

        private static void FindOrCreateFoldersAndFiles()
        {
            Directory.CreateDirectory(ManagerPath + @"\Addons");
            Directory.CreateDirectory(ManagerPath + @"\Servers");
            Directory.CreateDirectory(ManagerPath + @"\Backups");

            string dllName = FRAMEWORKFILENAME + ".dll";
            string dllSourceFilePath = Path.Combine(App.AppDirectory, dllName);
            string dllDestinationFilePath = Path.Combine(ManagerPath, dllName);

            File.Copy(dllSourceFilePath, dllDestinationFilePath, true);

            string xmlName = FRAMEWORKFILENAME + ".xml";
            string xmlSourceFilePath = Path.Combine(App.AppDirectory, xmlName);
            string xmlDestinationFilePath = Path.Combine(ManagerPath, xmlName);

            File.Copy(xmlSourceFilePath, xmlDestinationFilePath, true);
        }

        #region Addons
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
        private static async Task<List<Tuple<string, Config>>> InitializeConfigFiles()
        {
            Trace.WriteLine($"ConfigLoader: Loading server configs.");

            FileInfo[] configFiles = await GetConfigFiles();
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

        private static async Task<FileInfo[]> GetConfigFiles()
        {
            List<FileInfo> configFileInfos = new();

            string[] serverDirectories = Directory.GetDirectories(ServersPath);
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
                InitializeServer(config);
            }

            Initialized?.Invoke(typeof(Initializer), EventArgs.Empty);
        }
        public static void InitializeServer(Tuple<string, Config> serverConfig)
        {
            string typeName = serverConfig.Item2.FindValue("type");

            if (typeName == null)
            {
                InitializeConfigFile(serverConfig.Item1);
                typeName = nameof(HasDirectory);
            }

            string directoryPath = Path.GetDirectoryName(serverConfig.Item1);

            if (typeName == nameof(HasDirectory))
            {
                HasDirectoryList.AddServer(new HasDirectory(directoryPath));
                return;
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
                    if (serverType.IsAssignableTo(typeof(IHasDirectory)))
                    {
                        IHasDirectory hasDirectory = Activator.CreateInstance(serverType, directoryPath) as IHasDirectory;

                        HasDirectoryList.AddServer(hasDirectory);
                        typeFound = true;

                        break;
                    }
                }
            }

            if (!typeFound)
            {
                typeName = null;

                Trace.WriteLine("No type for " + Path.GetFileName(directoryPath) + " found");
                IHasDirectory hasDirectory = new HasDirectory(directoryPath);
                HasDirectoryList.AddServer(hasDirectory);
            }
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
