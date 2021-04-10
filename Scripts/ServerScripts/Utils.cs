using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public static class Extensions
    {
        public static T Last<T>(this IList<T> list) => list[list.Count - 1];
    }
}
