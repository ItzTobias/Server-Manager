using ServerManagerFramework;

namespace Server_Manager.Scripts
{
    public class HasDirectory : IHasDirectory
    {
        public string Directory { get; }

        public HasDirectory(string directory)
        {
            Directory = directory;
        }
    }
}
