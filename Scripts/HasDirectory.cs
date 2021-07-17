using ServerManagerFramework.Config;
using ServerManagerFramework.Servers;

namespace Server_Manager.Scripts
{
    public class HasDirectory : IHasDirectory
    {
        public string Directory { get; init; }

        public Config Config { get; init; }

        public void Initialized() { }
    }
}
