using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_Manager.Scripts.ServerScripts
{
    class LinesUpdateEventArgs : EventArgs
    {
        public string line;
        public bool replaceLast = false;

        public LinesUpdateEventArgs(string line) => this.line = line;
        public LinesUpdateEventArgs(string line, bool replaceLast)
        {
            this.line = line;
            this.replaceLast = replaceLast;
        }
    }
}
