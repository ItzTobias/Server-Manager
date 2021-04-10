using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_Manager.Scripts.ServerScripts
{
    public class StartArgs
    {
        readonly List<string> args = new List<string>(new string[1] { "-jar" });

        public void ChangeJarName(string name)
        {
            if (args.Count > 1) args[1] = name;
            else args.Add(name);
        }

        public void AddArg(string arg) => args.Add(arg);

        public void RemoveArg(string arg)
        {
            int index = args.FindIndex(o => o == arg);
            if (index != -1) args.RemoveAt(index);
        }

        public string ToArg()
        {
            string argLine = "";

            foreach (var arg in args) argLine += (' ' + arg);

            return argLine;
        }
    }
}
