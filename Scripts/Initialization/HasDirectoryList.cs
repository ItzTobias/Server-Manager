using ServerManagerFramework.Servers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace Server_Manager.Scripts.Initialization
{
    public class HasDirectoryList
    {
        private Type currentFilter = typeof(IHasDirectory);
        private readonly List<IHasDirectory> servers = new();
        private readonly List<InstallingServer> installingServers = new();
        public ObservableCollection<IHasDirectory> Servers { get; private set; } = new();

        public HasDirectoryList()
        {
            Application.Current.Exit += delegate
            {
                foreach (IHasDirectory server in servers)
                {
                    string path = Path.Combine(server.Directory, Initializer.CONFIGFILENAME);

                    _ = File.WriteAllTextAsync(path, server.Config.ToString());
                }
            };
        }

        public void AddServer(IHasDirectory server)
        {
            if (server is InstallingServer installingServer)
            {
                installingServers.Add(installingServer);

                if (installingServer.NewServer.GetType().IsAssignableTo(currentFilter))
                {
                    Servers.Add(installingServer);
                }

                installingServer.Installed += delegate
                {
                    installingServers.Remove(installingServer);
                    Servers.Remove(server);
                };

                return;
            }

            servers.Add(server);

            if (server.GetType().IsAssignableTo(currentFilter))
            {
                Servers.Add(server);
            }
        }

        public void RemoveServer(IHasDirectory server)
        {
            servers.Remove(server);

            if (currentFilter.Equals(server.GetType()))
            {
                Servers.Remove(server);
            }
        }

        public void Filter(Type serverType)
        {
            List<IHasDirectory> filteredProcesses = servers.FindAll(t => t.GetType().IsAssignableTo(serverType));
            filteredProcesses.AddRange(installingServers.FindAll(t => t.NewServer.GetType().IsAssignableTo(serverType)));

            Servers.Clear();
            filteredProcesses.ForEach(Servers.Add);

            currentFilter = serverType;
        }

        public void Clear()
        {
            servers.Clear();
            Filter(currentFilter);
        }
    }
}
