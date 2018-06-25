using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace MeshEditor.Extensions
{
    public static class ListExtensions
    {
        public static T Last<T>(this List<T> list)
        {
            return list[list.Count - 1];
        }


        public static T Next<T>(this List<T> list, T item)
        {
            var index = list.IndexOf(item);

            if (index == -1)
            {
                throw new Exception("Element not found at the list.");
            }

            if (index + 1 == list.Count)
            {
                return list.First();
            }

            return list[index + 1];
        }

        public static T Previous<T>(this List<T> list, T item)
        {
            var index = list.IndexOf(item);

            if (index == -1)
            {
                throw new Exception("Element not found at the list.");
            }

            if (index == 0)
            {
                return list.Last();
            }

            return list[index - 1];
        }

        public static int NextIndex<T>(this List<T> list, int index)
        {
            if (index < 0 || index >= list.Count)
            {
                throw new Exception("Index outside of the list.");
            }

            if (index + 1 == list.Count)
            {
                return 0;
            }

            return index + 1;
        }

        public static int PreviousIndex<T>(this List<T> list, int index)
        {
            if (index < 0 || index >= list.Count)
            {
                throw new Exception("Index outside of the list.");
            }

            if (index == 0)
            {
                return list.Count - 1;
            }

            return index - 1;
        }
    }
}