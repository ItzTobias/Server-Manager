using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class AppInfo
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

public enum ServerType
{
    Vanilla,
    Forge,
    Fabric,
    Spigot,
    Bukkit
}