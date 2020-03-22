using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sibz.ChildSubset
{
    public class ChildSubset : ChildSubset<GameObject>
    {
        public ChildSubset(GameObject rootObject) : base(rootObject)
        {

        }
        public enum SortDir
        {
            Ascending,
            Descending
        }

    }
    public class ChildSubset<T>
        where T: Object
    {
        public GameObject RootObject { get; }
        public Func<T, bool> Filter { get; set; }
        public Func<T, IComparable> Sort { get; set; }
        public bool Recursive { get; set; } = true;
        public ChildSubset.SortDir SortDirection { get; set; } = ChildSubset.SortDir.Ascending;

        public ChildSubset(GameObject rootObject)
        {
            if (rootObject is null)
            {
                throw new ArgumentNullException(nameof(rootObject));
            }
            RootObject = rootObject;
        }

        public int Count => GetChildren(RootObject, Filter, Sort, Recursive, SortDirection).Count();
        public IEnumerable<T> List => GetChildren(RootObject, Filter, Sort, Recursive, SortDirection);

        public static IEnumerable<T> GetChildren(GameObject rootObject, Func<T, bool> filter = null, Func<T, IComparable> sort = null, bool recursive = true, ChildSubset.SortDir sortDir = ChildSubset.SortDir.Ascending)
        {
            if (rootObject is null)
            {
                throw new ArgumentNullException(nameof(rootObject));
            }

            filter = filter ?? (x => true);

            var results = new List<T>();
            for (int childIndex = 0; childIndex < rootObject.transform.childCount; childIndex++)
            {
                GameObject result = rootObject.transform.GetChild(childIndex).gameObject;
                object monoResult = typeof(T).IsSubclassOf(typeof(MonoBehaviour))
                    ? result.GetComponent<T>()
                    : (Object)result;

                if (result != rootObject && (monoResult is T r) && filter.Invoke(r))
                {
                    results.Add(r);
                }
                if (recursive)
                {
                    results.AddRange(GetChildren(result, filter));
                }
            }

            return (sort is null) ? (IEnumerable<T>) results : sortDir == ChildSubset.SortDir.Ascending ? results.OrderBy(sort) : results.OrderByDescending(sort);
        }
    }
}