namespace Server_Manager.Scripts.ServerScripts
{
    public class NameValuePair
    {
        private string name;
        public string Name
        {
            get
            {
                string formattedName = name;

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

        public string GetNameUnformatted()
        {
            return name;
        }
    }
}
