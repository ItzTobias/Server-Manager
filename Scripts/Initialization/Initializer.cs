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

        public static event EventHandler AddonsLoaded;

        public static async Task Initialize()
        {
            FindOrCreateFoldersAndFiles();

            LoadAllAddons();

            //Initialize Servers
            List<Task> initializeServerTasks = new();
            foreach (string path in Directory.EnumerateDirectories(ServersPath))
            {
                initializeServerTasks.Add(InitializeServer(path));
            }
            await Task.WhenAll(initializeServerTasks);
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

            AddonsLoaded?.Invoke(typeof(Initializer), EventArgs.Empty);
        }

        public static async Task InitializeServer(string path)
        {
            //Load Config File
            string configFilePath = Path.Combine(path, CONFIGFILENAMES);

            if (!File.Exists(configFilePath))
            {
                await File.WriteAllTextAsync(configFilePath, DefaultConfig);
            }

            string configFileContent = await File.ReadAllTextAsync(configFilePath);
            Config config = new(configFileContent);


            //Initialize Server
            IHasDirectory hasDirectory = new HasDirectory(path);

            string serverTypeName = config.FindValue("type");

            if (serverTypeName == null)
            {
                await File.WriteAllTextAsync(configFilePath, DefaultConfig);
                serverTypeName = nameof(HasDirectory);
            }

            if (serverTypeName != nameof(HasDirectory))
            {
                bool typeFound = false;

                foreach (Assembly addon in addons)
                {
                    Type serverType = addon.GetType(serverTypeName);

                    if (serverType == null)
                    {
                        continue;
                    }
                    else
                    {
                        if (!serverType.IsAssignableTo(typeof(IHasDirectory)))
                        {
                            continue;
                        }

                        ConstructorInfo serverTypeConstructor = serverType.GetConstructor(new Type[1] { typeof(string) });

                        if (serverTypeConstructor == null)
                        {
                            continue;
                        }

                        hasDirectory = serverTypeConstructor.Invoke(new object[1] { path }) as IHasDirectory;

                        typeFound = true;

                        break;
                    }
                }

                if (!typeFound)
                {
                    Trace.WriteLine("No type for " + Path.GetFileName(path) + " found");

                    hasDirectory = new HasDirectory(path);
                }
            }

            HasDirectoryList.AddServer(hasDirectory);
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
    }
}
