using System;
using System.Collections.Generic;
using System.IO;

namespace Server_Manager.Scripts.ServerScripts
{
    public static class ServerCollections
    {
        public static Vanilla[] Vanilla { get; private set; } = new Vanilla[0];
        public static Server[] Forge { get; private set; } = new Vanilla[0];
        public static Server[] Fabric { get; private set; } = new Vanilla[0];
        public static Server[] Bukkit { get; private set; } = new Vanilla[0];

        public static EventHandler<CollectionUpdateEventArgs> onCollectionUpdate;

        public static void UpdateAll()
        {
            UpdateVanilla();
            UpdateForge();
            UpdateFabric();
            UpdateBukkit();
        }
        public static void UpdateVanilla()
        {
            List<Vanilla> servers = new List<Vanilla>();

            string[] vanillaServers = Directory.GetDirectories(new Vanilla("", -1).ParentDirectory);

            for (int i = 0; i < vanillaServers.Length; i++)
                servers.Add(new Vanilla(Path.GetFileName(vanillaServers[i]), i));

            Vanilla = servers.ToArray();

            onCollectionUpdate?.Invoke(typeof(ServerCollections), new CollectionUpdateEventArgs(CollectionType.Vanilla));
        }
        public static void UpdateForge()
        {
            onCollectionUpdate?.Invoke(typeof(ServerCollections), new CollectionUpdateEventArgs(CollectionType.Forge));
        }
        public static void UpdateFabric()
        {
            onCollectionUpdate?.Invoke(typeof(ServerCollections), new CollectionUpdateEventArgs(CollectionType.Fabric));
        }
        public static void UpdateBukkit()
        {
            onCollectionUpdate?.Invoke(typeof(ServerCollections), new CollectionUpdateEventArgs(CollectionType.Bukkit));
        }
    }
}
