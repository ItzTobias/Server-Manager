using System.Collections.ObjectModel;

namespace Library
{
    public static class Utils
    {
        public static T Last<T>(this ObservableCollection<T> collection)
        {
            return collection[collection.Count - 1];
        }
    }
}
