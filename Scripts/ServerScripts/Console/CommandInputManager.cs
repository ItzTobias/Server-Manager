using System.Collections.Generic;

namespace Server_Manager.Scripts.ServerScripts
{
    public class CommandInputManager
    {
        private int index = 1;
        private readonly List<string> commands = new();
        private bool canEdit = true;

        public CommandInputManager()
        {
            NewCommand();
        }

        public string GetCurrentCommand()
        {
            return commands[^index];
        }

        public void NewCommand()
        {
            commands.Add("");
        }

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
            if (index < commands.Count)
            {
                index++;
            }

            if (index != 1)
            {
                canEdit = false;
            }
        }
        public void GoForward()
        {
            if (index > 1)
            {
                index--;
            }

            if (index != 1)
            {
                canEdit = false;
            }
        }
    }
}
