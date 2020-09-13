using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class AppInfo
{
    /// <summary>
    /// Path for resources inside the project folder
    /// </summary>
    public const string dataPath = "pack://application:,,,/";
}

public enum ServerType
{
    Vanilla,
    Forge,
    Fabric,
    Spigot,
    Bukkit
}
