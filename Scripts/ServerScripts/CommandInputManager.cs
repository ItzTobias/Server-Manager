using System.Collections.Generic;
using System.Diagnostics;

namespace Server_Manager.Scripts.ServerScripts
{
    public class CommandInputManager
    {
        int index = 1;
        readonly List<string> commands = new List<string>();
        bool canEdit = true;

        public CommandInputManager() => NewCommand();

        public string GetCurrentCommand() => commands[^index];
        public void NewCommand() => commands.Add("");
        public void EditCurrent(string command)
        {
            if (canEdit)
            {
                commands[^1] = command;
                index = 1;
            }

            canEdit = true;
        }
        public void GoBack()
        {
            if (index < commands.Count) index++;
            if (index != 1) canEdit = false;
        }
        public void GoForward()
        {
            if (index > 1) index--;
            if (index != 1) canEdit = false;
        }
    }
}
