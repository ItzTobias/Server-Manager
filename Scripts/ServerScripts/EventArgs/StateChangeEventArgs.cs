using System;

namespace Server_Manager.Scripts.ServerScripts
{
    public class StateChangeEventArgs : EventArgs
    {
        public State serverState;

        public StateChangeEventArgs(State serverState)
        {
            this.serverState = serverState;
        }
    }
}
