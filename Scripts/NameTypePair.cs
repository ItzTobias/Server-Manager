using System;

namespace Server_Manager.Scripts
{
    public class NameTypePair
    {
        private readonly string name;
        public Type Type { get; }

        public NameTypePair(string name, Type type)
        {
            this.name = name;
            Type = type;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
