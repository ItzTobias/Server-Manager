using System;

namespace Server_Manager.Scripts.ServerScripts
{
    public class StateChangeEventArgs : EventArgs
    {
        public State_old serverState;

        public StateChangeEventArgs(State_old serverState)
        {
            this.serverState = serverState;
        }
    }
}
