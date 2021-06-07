using ServerManagerFramework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Server_Manager.Scripts.Initialization
{
    //ComboBoxButtonAttribute in Initializer.InitializeComboBox()
    public class HasDirectoryList
    {
        private Type currentFilter = typeof(IHasDirectory);
        private readonly List<IHasDirectory> serverProcesses = new();
        public ObservableCollection<IHasDirectory> ServerProcesses { get; private set; } = new();

        public void AddServer(IHasDirectory server)
        {
            serverProcesses.Add(server);

            if (server.GetType().IsAssignableTo(currentFilter))
            {
                ServerProcesses.Add(server);
            }
        }

        public void RemoveServer(IHasDirectory server)
        {
            serverProcesses.Remove(server);

            if (currentFilter.Equals(server.GetType()))
            {
                ServerProcesses.Remove(server);
            }
        }

        public bool Contains(IHasDirectory server)
        {
            return serverProcesses.Contains(server);
        }

        public void ResetFilter()
        {
            ServerProcesses.Clear();
            serverProcesses.ForEach(ServerProcesses.Add);
            currentFilter = typeof(IHasDirectory);
        }

        public void Filter<T>()
        {
            List<IHasDirectory> filteredProcesses = serverProcesses.FindAll(t => t is T);

            ServerProcesses.Clear();
            filteredProcesses.ForEach(ServerProcesses.Add);

            currentFilter = typeof(T);
        }

        public void Filter(Type serverType)
        {
            List<IHasDirectory> filteredProcesses = serverProcesses.FindAll(t => t.GetType() == serverType);

            ServerProcesses.Clear();
            filteredProcesses.ForEach(ServerProcesses.Add);

            currentFilter = serverType;
        }
    }
}
