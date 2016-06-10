using System;
using System.Collections.Generic;
using System.Linq;

namespace CarouselSample.Toolbox
{
    public static class CollectionToolbox
    {
        /// <summary>
        /// Inserts the items into the list in a sorted order defined by comparer. 
        /// It's assumed that the list is already sorted by the same sort order as produced by comparer.
        /// If the comparer is null all the items are appended at the end of the list.
        /// </summary>
        /// <param name="list">List.</param>
        /// <param name="items">Items.</param>
        /// <param name="comparer">Comparer.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void InsertSorted<T>(this IList<T> list, IEnumerable<T> items, Comparison<T> comparer = null)
        {
            if (items == null)
                return;
            list = list ?? new List<T>();
            if (comparer == null)
            {
                foreach (var obj in items)
                    list.Add(obj);
            }
            else
            {
                var sorted = items.ToList();
                sorted.Sort(comparer);
                var j = 0;
                foreach (var item in sorted)
                {
                    var comparison = 0;
                    //Find the place where the item should be inserted
                    while (j < list.Count && (comparison = comparer.Invoke(list[j], item)) < 0)
                        j++;
                    //If they are equal, simply update the item
                    if (j < list.Count && comparison == 0)
                        list[j] = item;
                    //Otherwise insert the new item in new place
                    else
                        list.Insert(j, item);
                    j++;
                }
            }
        }

        /// <summary>
        /// Adds each element from items to collection.
        /// </summary>
        /// <param name="collection">Collection.</param>
        /// <param name="items">Items.</param>
        /// <typeparam name="T">The type of collection.</typeparam>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (items == null)
                return;
            foreach (var item in items)
                collection.Add(item);
        }

        public static void InsertRange<T>(this IList<T> list, int index, IEnumerable<T> items)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            if (items == null)
                throw new ArgumentNullException("items");
            if (index < 0 || index > list.Count)
                throw new IndexOutOfRangeException();
            var j = index;
            foreach (var item in items)
            {
                list.Insert(j++, item);
            }
        }
    }
}