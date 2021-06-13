using Server_Manager.Scripts.Initialization;
using ServerManagerFramework;
using System.Windows;

namespace Server_Manager
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            GlobalConfig.Load();
            Initializer.Initialize();
        }
    }
}
