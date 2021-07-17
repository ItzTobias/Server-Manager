using Microsoft.Win32;
using Server_Manager.Views;
using ServerManagerFramework;
using ServerManagerFramework.Addons;
using ServerManagerFramework.Config;
using ServerManagerFramework.Installing;
using ServerManagerFramework.Servers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace Server_Manager.Scripts.Initialization
{
    public static class Initializer
    {
        public static string AddonsPath => GlobalConfig.ManagerPath + @"\Addons";

        private const string FRAMEWORKFILENAME = "ServerManagerFramework";

        private static readonly List<Assembly> addons = new();

        public const string CONFIGFILENAME = "server-manager.config";
        public const string DONTLOADFILENAME = "dont-load.lock";

        private static string DefaultConfig => "type=" + nameof(HasDirectory);

        public static HasDirectoryList HasDirectoryList { get; private set; } = new();
        public static ObservableCollection<NameTypePair> ComboBoxItems { get; } = new();
        public static Dictionary<string, Installable> Installables { get; private set; }

        public static void Initialize()
        {
            FindOrCreateFoldersAndFiles();

#pragma warning disable 1998
            static async IAsyncEnumerable<Progress> installDefaultServer(string path)
            {
                yield return new Progress();
            }
#pragma warning restore

            Installable installable = new(Array.Empty<UIElement>(), typeof(HasDirectory), installDefaultServer);

            ServerManagerFramework.Installing.Installables.SetInstallable("Default", installable);

            LoadAllAddons();

            LoadInstallables();
        }

        private static void LoadInstallables()
        {
            FieldInfo installablesField = typeof(Installables).GetField("installables", BindingFlags.NonPublic | BindingFlags.Static);
            Installables = installablesField.GetValue(null) as Dictionary<string, Installable>;
        }

        private static void FindOrCreateFoldersAndFiles()
        {
            Directory.CreateDirectory(AddonsPath);
            Directory.CreateDirectory(GlobalConfig.ServersPath);
            Directory.CreateDirectory(GlobalConfig.BackupsPath);

            string dllName = FRAMEWORKFILENAME + ".dll";
            string dllSourceFilePath = Path.Combine(SMR.AppDirectory, dllName);
            string dllDestinationFilePath = Path.Combine(GlobalConfig.ManagerPath, dllName);

            File.Copy(dllSourceFilePath, dllDestinationFilePath, true);

            string xmlName = FRAMEWORKFILENAME + ".xml";
            string xmlSourceFilePath = Path.Combine(SMR.AppDirectory, xmlName);
            string xmlDestinationFilePath = Path.Combine(GlobalConfig.ManagerPath, xmlName);

            File.Copy(xmlSourceFilePath, xmlDestinationFilePath, true);
        }
        private static void LoadAllAddons()
        {
            addons.Clear();

            Trace.WriteLine($"AddonsLoader: Loading addons.");

            List<string> assemblyLocations = new();
            List<Assembly> allAddons = new();

            string[] addonPaths = Directory.GetFiles(AddonsPath);

            foreach (string addonPath in addonPaths)
            {
                if (Path.GetExtension(addonPath) != ".dll")
                {
                    continue;
                }

                Assembly addonAssembly = Assembly.LoadFrom(addonPath);

                if (assemblyLocations.Contains(addonAssembly.Location))
                {
                    continue;
                }

                assemblyLocations.Add(addonAssembly.Location);

                allAddons.Add(addonAssembly);
            }

            List<ErrorItem> errorList = new();
            Style buttonStyle = Application.Current.Resources["GreenButton"] as Style;
            Style downloadingButtonStyle = Application.Current.Resources["GrayButton"] as Style;
            List<MethodInfo> methodsToExecute = new();

            foreach (Assembly addon in allAddons)
            {
                bool errorThrown = false;

                foreach (RequireAttribute requiredItem in addon.GetCustomAttributes<RequireAttribute>())
                {
                    Version itemVersion = null;
                    switch (requiredItem.ItemType)
                    {
                        case ItemType.ExeInstaller:
                            itemVersion = FindProgram(requiredItem.ItemName);
                            break;
                        case ItemType.MsiInstaller:
                            itemVersion = FindProgram(requiredItem.ItemName);
                            break;
                        case ItemType.Addon:
                            itemVersion = allAddons.Find(a => a.GetName().Name == requiredItem.ItemName)?.GetName().Version;
                            break;
                        default:
                            Trace.WriteLine("RequiredItem.ItemType not found. " + requiredItem.ItemType.ToString());
                            break;
                    }

                    ErrorReason? errorReason = null;

                    if (itemVersion == null)
                    {
                        errorReason = ErrorReason.missing;
                    }
                    else if (requiredItem.MinVersion != null && itemVersion < new Version(requiredItem.MinVersion))
                    {
                        errorReason = ErrorReason.tooOld;
                    }
                    else if (requiredItem.MaxVersion != null && itemVersion > new Version(requiredItem.MaxVersion))
                    {
                        errorReason = ErrorReason.tooNew;
                    }

                    if (errorReason == null)
                    {
                        continue;
                    }

                    if (!errorThrown)
                    {
                        errorThrown = true;
                    }

                    string itemTypeName = requiredItem.ItemType switch
                    {
                        ItemType.ExeInstaller => "program",
                        ItemType.MsiInstaller => "program",
                        ItemType.Addon => "addon",
                        _ => "item"
                    };
                    string errorReasonName = errorReason switch
                    {
                        ErrorReason.missing => "missing",
                        ErrorReason.tooOld => "outdated",
                        ErrorReason.tooNew => "too new or this plugin is outdated",
                        _ => "missing."
                    };

                    ErrorItem errorItem = new()
                    {
                        ErrorMessage = $"Can't load {addon.GetName().Name} because the {itemTypeName} {requiredItem.ItemName} is {errorReasonName}."
                    };

                    if (requiredItem.DownloadURL != null)
                    {
                        errorItem.ButtonVisibility = Visibility.Visible;
                        errorItem.ButtonStyle = buttonStyle;
                        errorItem.ButtonText = errorReason switch
                        {
                            ErrorReason.missing => "Install",
                            ErrorReason.tooOld => "Update",
                            ErrorReason.tooNew => "Downgrade",
                            _ => "Install"
                        };

                        errorItem.ButtonClickAction = new ErrorItemCommand(DownloadItem);
                    }

                    errorList.Add(errorItem);

                    async void DownloadItem()
                    {
                        errorItem.ButtonEnabled = false;
                        errorItem.ButtonText = errorReason switch
                        {
                            ErrorReason.missing => "Installing",
                            ErrorReason.tooOld => "Updating",
                            ErrorReason.tooNew => "Downgrading",
                            _ => "Installing"
                        }; ;
                        errorItem.ButtonStyle = downloadingButtonStyle;

                        string filePath = "";

                        void Failed()
                        {
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }

                            errorItem.ButtonEnabled = true;
                            errorItem.ButtonText = errorReason switch
                            {
                                ErrorReason.missing => "Install",
                                ErrorReason.tooOld => "Update",
                                ErrorReason.tooNew => "Downgrade",
                                _ => "Install"
                            };
                            errorItem.ButtonStyle = buttonStyle;
                            if (!errorItem.ErrorMessage.Contains('|'))
                            {
                                errorItem.ErrorMessage += " | An error occured while installing. \nDownload manually: " + requiredItem.DownloadURL;
                            }
                        }
                        void Succeeded()
                        {
                            errorItem.ButtonVisibility = Visibility.Hidden;
                            errorItem.ErrorMessage = errorReason switch
                            {
                                ErrorReason.missing => $"Installed {requiredItem.ItemName} successfully",
                                ErrorReason.tooOld => $"Updated {requiredItem.ItemName} successfully",
                                ErrorReason.tooNew => $"Downgraded {requiredItem.ItemName} successfully",
                                _ => $"Installed {requiredItem.ItemName} successfully"
                            };
                        }

                        try
                        {
                            HttpWebRequest request = WebRequest.CreateHttp(requiredItem.DownloadURL);
                            using HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;
                            using Stream responseStream = response.GetResponseStream();

                            string fileName = response.Headers["Content-Disposition"]?.Split(new string[] { "=" }, StringSplitOptions.None)[1];

                            if (fileName == null)
                            {
                                fileName = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                                fileName = fileName.Replace("=", "-");
                                fileName = fileName.Replace("+", "_");
                                fileName += requiredItem.ItemType switch
                                {
                                    ItemType.ExeInstaller => ".exe",
                                    ItemType.MsiInstaller => ".msi",
                                    ItemType.Addon => ".dll",
                                    _ => throw new Exception("Can't download a file for a missing ItemType")
                                };
                            }

                            filePath = Path.Combine(AddonsPath, fileName);

                            FileStream fileStream = File.Create(filePath);
                            await responseStream.CopyToAsync(fileStream);
                            await fileStream.DisposeAsync();
                        }
                        catch
                        {
                            Failed();
                            return;
                        }

                        if (requiredItem.ItemType == ItemType.Addon)
                        {
                            Succeeded();
                            return;
                        }

                        Process installer = new()
                        {
                            EnableRaisingEvents = true
                        };
                        installer.StartInfo.FileName = filePath;
                        installer.StartInfo.Verb = "runas";
                        installer.StartInfo.UseShellExecute = true;
                        void ProcessEnded()
                        {
                            installer.Dispose();
                            File.Delete(filePath);

                            if (FindProgram(requiredItem.ItemName) == null)
                            {
                                Failed();
                            }
                            else
                            {
                                Succeeded();
                            }
                        }
                        installer.Exited += delegate
                        {
                            ProcessEnded();
                        };

                        try
                        {
                            installer.Start();
                        }
                        catch
                        {
                            ProcessEnded();
                        }
                    }
                }

                if (!errorThrown)
                {
                    addons.Add(addon);
                }

                Type entryPoint = (from type
                                  in addon.GetTypes()
                                   where type.IsAbstract && type.IsSealed && type.Name == "EntryPoint"
                                   select type).FirstOrDefault();

                if (entryPoint == null)
                {
                    continue;
                }

                MethodInfo addonLoaded = entryPoint.GetMethod("AddonLoaded");
                if (addonLoaded != null
                    && addonLoaded.IsStatic
                    && addonLoaded.GetParameters().Length == 0
                    && !addonLoaded.IsGenericMethod
                    && !addonLoaded.IsAbstract)
                {
                    addonLoaded.Invoke(null, null);
                }

                MethodInfo addonsLoaded = entryPoint.GetMethod("AddonsLoaded");
                if (addonsLoaded != null
                    && addonsLoaded.IsStatic
                    && addonsLoaded.GetParameters().Length == 0
                    && !addonsLoaded.IsGenericMethod
                    && !addonsLoaded.IsAbstract)
                {
                    methodsToExecute.Add(addonsLoaded);
                }
            }

            Trace.WriteLine("AddonsLoader: Loaded " + addons.Count + " addons.");

            void Finish()
            {
                foreach (MethodInfo method in methodsToExecute)
                {
                    method.Invoke(null, null);
                }

                AddonsLoaded();
            }

            if (errorList.Count == 0)
            {
                Finish();
                return;
            }
            else
            {
                _ = MainWindow.GetMainWindow.ShowErrorMessage(new ErrorMessage()
                {
                    Text = "There were errors while loading some addons",
                    ErrorItems = errorList,
                    GreenButton = new Tuple<string, RoutedEventHandler>("Reload addons", delegate
                    {
                        MainWindow.GetMainWindow.HideErrorMessage();
                        LoadAllAddons();
                    }),
                    GrayButton = new Tuple<string, RoutedEventHandler>("Run anyways", delegate
                    {
                        MainWindow.GetMainWindow.HideErrorMessage();
                        Finish();
                    }),
                    RedButton = new Tuple<string, RoutedEventHandler>("Quit", delegate
                    {
                        Application.Current.Shutdown();
                    })
                });
            }
        }
        private static Version FindProgram(string programName)
        {
            string displayName;

            static Version FoundProgram(RegistryKey subkey)
            {
                string versionString = subkey.GetValue("DisplayVersion") as string;
                return new Version(versionString);
            }

            string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey);
            if (key != null)
            {
                foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                {
                    displayName = subkey.GetValue("DisplayName") as string;
                    if (displayName != null && displayName.Contains(programName))
                    {
                        return FoundProgram(subkey);
                    }
                }
                key.Close();
            }

            registryKey = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
            key = Registry.LocalMachine.OpenSubKey(registryKey);
            if (key != null)
            {
                foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                {
                    displayName = subkey.GetValue("DisplayName") as string;
                    if (displayName != null && displayName.Contains(programName))
                    {
                        return FoundProgram(subkey);
                    }
                }
                key.Close();
            }
            return null;
        }

        private static void AddonsLoaded()
        {
            InitializeComboBox();

            _ = InitializeServers();
        }

        public static async Task InitializeServer(string path)
        {
            //Load Config File
            string configFilePath = Path.Combine(path, CONFIGFILENAME);

            if (!File.Exists(configFilePath))
            {
                await File.WriteAllTextAsync(configFilePath, DefaultConfig);
            }

            string configFileContent = await File.ReadAllTextAsync(configFilePath);
            Config config = new(configFileContent);

            //Initialize Server
            IHasDirectory hasDirectory = new HasDirectory();

            string serverTypeName = config.GetValue("type");

            if (serverTypeName == null)
            {
                await File.WriteAllTextAsync(configFilePath, DefaultConfig);
                serverTypeName = nameof(HasDirectory);
            }

            if (serverTypeName != nameof(HasDirectory))
            {
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

                        ConstructorInfo serverTypeConstructor = serverType.GetConstructor(Array.Empty<Type>());

                        if (serverTypeConstructor == null)
                        {
                            continue;
                        }

                        hasDirectory = serverTypeConstructor.Invoke(Array.Empty<object>()) as IHasDirectory;

                        break;
                    }
                }
            }

            PropertyInfo directoryProperty = typeof(IHasDirectory).GetProperty(nameof(IHasDirectory.Directory));
            directoryProperty.SetValue(hasDirectory, path);

            PropertyInfo configProperty = typeof(IHasDirectory).GetProperty(nameof(IHasDirectory.Config));
            configProperty.SetValue(hasDirectory, config);

            HasDirectoryList.AddServer(hasDirectory);

            MethodInfo initializedMethod = typeof(IHasDirectory).GetMethod("Initialized");
            initializedMethod.Invoke(hasDirectory, null);
        }

        public static async Task InitializeServers()
        {
            HasDirectoryList.Clear();

            //Initialize Servers
            List<Task> initializeServerTasks = new();
            foreach (string path in Directory.EnumerateDirectories(GlobalConfig.ServersPath))
            {
                string filePath = Path.Combine(path, DONTLOADFILENAME);

                if (File.Exists(filePath))
                {
                    continue;
                }

                initializeServerTasks.Add(InitializeServer(path));
            }
            await Task.WhenAll(initializeServerTasks);
        }

        public static async Task CreateServer(string path, Type serverType, InstallAction installAction)
        {
            Directory.CreateDirectory(path);

            string filePath = Path.Combine(path, DONTLOADFILENAME);
            Stream stream = File.Create(filePath);

            //Load Config File
            string configFilePath = Path.Combine(path, CONFIGFILENAME);

            Config config = new("type=" + serverType.FullName);
            await File.WriteAllTextAsync(configFilePath, config.ToString());

            //Initialize Server
            ConstructorInfo serverTypeConstructor = serverType.GetConstructor(Array.Empty<Type>());

            IHasDirectory hasDirectory = serverTypeConstructor.Invoke(Array.Empty<object>()) as IHasDirectory;

            InstallingServer installingServer = new()
            {
                Directory = path,
                Config = config,
                NewServer = hasDirectory
            };

            HasDirectoryList.AddServer(installingServer);

            await foreach (Progress progress in installAction(path))
            {
                installingServer.Text = progress.Text;
                installingServer.Percentage = progress.Percentage;
            }

            installingServer.InvokeInstalled();

            PropertyInfo directoryProperty = typeof(IHasDirectory).GetProperty(nameof(IHasDirectory.Directory));
            directoryProperty.SetValue(hasDirectory, path);

            PropertyInfo configProperty = typeof(IHasDirectory).GetProperty(nameof(IHasDirectory.Config));
            configProperty.SetValue(hasDirectory, config);

            HasDirectoryList.AddServer(hasDirectory);

            MethodInfo initializedMethod = typeof(IHasDirectory).GetMethod("Initialized");
            initializedMethod.Invoke(hasDirectory, null);

            await stream.DisposeAsync();
            File.Delete(filePath);
        }

        private static void InitializeComboBox()
        {
            Trace.WriteLine($"ComboBoxInitializer: Initializing combobox.");

            ComboBoxItems.Add(new NameTypePair("All", typeof(IHasDirectory)));
            ComboBoxItems.Add(new NameTypePair("Unknown", typeof(HasDirectory)));

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
                        if (!serverType.IsAssignableTo(typeof(IHasDirectory)))
                        {
                            continue;
                        }

                        if (comboBoxButtonAttribute.ClassName == null)
                        {
                            ComboBoxItems.Add(new NameTypePair(serverType.Name, serverType));
                        }
                        else
                        {
                            ComboBoxItems.Add(new NameTypePair(comboBoxButtonAttribute.ClassName, serverType));
                        }
                    }
                }
            }

            Trace.WriteLine($"ComboBoxInitializer: Initialized {ComboBoxItems.Count} buttons.");
        }
    }
}
