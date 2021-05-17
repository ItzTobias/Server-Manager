using Server_Manager.Properties;
using System;
using System.IO;

namespace Server_Manager.Scripts.ServerScripts
{
    public class Vanilla : Server
    {
        public override string ParentDirectory => Path.Combine(Settings.Default.SERVERS_PATH, "Vanilla");

        public Vanilla(string name, int arrayIndex) : base(name, arrayIndex) { }

        public override void Install()
        {
            throw new NotImplementedException();
        }
    }
}
