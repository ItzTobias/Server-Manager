using System;

namespace Server_Manager.Scripts.ServerScripts
{
    public class CollectionUpdateEventArgs : EventArgs
    {
        public CollectionType collectionType;

        public CollectionUpdateEventArgs(CollectionType collectionType) => this.collectionType = collectionType;
    }
}
