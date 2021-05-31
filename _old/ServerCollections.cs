using Server_Manager.Properties;
using System;
using System.Collections.Generic;
using System.IO;

namespace Server_Manager.Scripts.ServerScripts
{
    public static class ServerCollections
    {
        public static Vanilla[] Vanilla { get; private set; } = Array.Empty<Vanilla>();
        public static Server[] Forge { get; private set; } = Array.Empty<Vanilla>();
        public static Server[] Fabric { get; private set; } = Array.Empty<Vanilla>();
        public static Server[] Bukkit { get; private set; } = Array.Empty<Vanilla>();

        public static EventHandler<CollectionUpdateEventArgs> OnCollectionUpdate { get => onCollectionUpdate; set => onCollectionUpdate = value; }

        private static EventHandler<CollectionUpdateEventArgs> onCollectionUpdate;

        public static void UpdateAll()
        {
            UpdateVanilla();
            UpdateForge();
            UpdateFabric();
            UpdateBukkit();
        }
        public static void UpdateVanilla()
        {
            List<Vanilla> servers = new();

            string[] vanillaServers = Directory.GetDirectories("SERVERS_PATH");

            for (int i = 0; i < vanillaServers.Length; i++)
            {
                servers.Add(new Vanilla(Path.GetFileName(vanillaServers[i]), i));
            }

            Vanilla = servers.ToArray();

            OnCollectionUpdate?.Invoke(typeof(ServerCollections), new CollectionUpdateEventArgs(CollectionType.Vanilla));
        }
        public static void UpdateForge()
        {
            OnCollectionUpdate?.Invoke(typeof(ServerCollections), new CollectionUpdateEventArgs(CollectionType.Forge));
        }
        public static void UpdateFabric()
        {
            OnCollectionUpdate?.Invoke(typeof(ServerCollections), new CollectionUpdateEventArgs(CollectionType.Fabric));
        }
        public static void UpdateBukkit()
        {
            OnCollectionUpdate?.Invoke(typeof(ServerCollections), new CollectionUpdateEventArgs(CollectionType.Bukkit));
        }
    }
}
