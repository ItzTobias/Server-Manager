using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_Manager.Scripts.ServerScripts
{
    public class NameValuePair
    {
        string name;
        public string Name
        {
            get
            {
                string formattedName = string.Copy(name);

                formattedName = formattedName.Replace('-', ' ');
                formattedName = formattedName.ToUpper();

                return formattedName;
            }
            set => name = value;
        }
        public string Value { get; set; }

        public NameValuePair(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string GetNameUnformatted() => name;
    }
}
