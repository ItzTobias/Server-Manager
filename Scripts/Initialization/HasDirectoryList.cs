using ServerManagerFramework;
using System;
using System.Collections.Generic;

namespace Server_Manager.Scripts.Initialization
{
    //ComboBoxButtonAttribute in Initializer.InitializeComboBox()
    public class HasDirectoryList
    {
        private List<IHasDirectory> ServerProcesses { get; } = new();

        public void AddServer(IHasDirectory server)
        {
            ServerProcesses.Add(server);
        }
        public void RemoveServer(IHasDirectory server)
        {
            ServerProcesses.Remove(server);
        }

        public bool Contains(IHasDirectory server)
        {
            return ServerProcesses.Contains(server);
        }

        public IHasDirectory[] GetServers<T>()
        {
            return ServerProcesses.FindAll(t => t.GetType() == typeof(T)).ToArray();
        }
        public IHasDirectory[] GetServers(Type serverType)
        {
            return ServerProcesses.FindAll(t => t.GetType() == serverType).ToArray();
        }

        public IHasDirectory[] GetServers()
        {
            return ServerProcesses.ToArray();
        }
    }
}
