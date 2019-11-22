using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sibz.EditorList
{
    /// <summary>
    /// Helper class to override for use with property drawers
    /// </summary>
    /// <typeparam name="T">Type of item in list</typeparam>
    public class EditorList<T> : ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IList
    {
        [SerializeField]
        public List<T> List = new List<T>();

        public T this[int index] { get => List[index]; set => List[index] = value; }
        object IList.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Count => List.Count;

        public bool IsReadOnly => ((IList)List).IsReadOnly;

        public bool IsFixedSize => ((IList)List).IsFixedSize;

        public bool IsSynchronized => ((IList)List).IsSynchronized;

        public object SyncRoot => ((IList)List).SyncRoot;

        public void Add(T item) => List.Add(item);

        public int Add(object value) => ((IList)List).Add(value);

        public void AddRange(IEnumerable<T> collection) => List.AddRange(collection);

        public void Clear() => List.Clear();

        public bool Contains(T item) => List.Contains(item);

        public bool Contains(object value) => ((IList)List).Contains(value);

        public void CopyTo(T[] array, int arrayIndex) => List.CopyTo(array, arrayIndex);

        public void CopyTo(Array array, int index) => ((IList)List).CopyTo(array, index);

        public IEnumerator<T> GetEnumerator() => List.GetEnumerator();

        public int IndexOf(T item) => List.IndexOf(item);

        public int IndexOf(object value) => ((IList)List).IndexOf(value);

        public void Insert(int index, T item) => List.Insert(index, item);

        public void Insert(int index, object value) => ((IList)List).Insert(index, value);

        public bool Remove(T item) => List.Remove(item);

        public void Remove(object value) => ((IList)List).Remove(value);

        public void RemoveAt(int index) => List.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => List.GetEnumerator();
    }
}
