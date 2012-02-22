using System;
using System.Collections.Generic;

namespace NClassify.Generator
{
    public static class Extensions
    {
        public static IEnumerable<T> SafeEnum<T>(this IEnumerable<T> e)
        {
            return e ?? new T[0];
        }

        public static void ForAll<T>(this IEnumerable<T> e, Action<T> a)
        {
            if (e != null)
                foreach (T i in e)
                    a(i);
        }
    }
}
