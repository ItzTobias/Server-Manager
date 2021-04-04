using System;
using System.Windows;

namespace Server_Manager
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Path for resources inside the project folder
        /// </summary>
        public static string DataPath { get => "pack://application:,,,/"; }
        /// <summary>
        /// Path to the folder where the Application is installed in
        /// </summary>
        public static string AppDirectory { get => AppDomain.CurrentDomain.BaseDirectory; }
    }
}
