using System;
using UnityEngine.UIElements;

namespace Sibz.ListElement
{
    public class ListRowElement : VisualElement
    {
        public ListRowElement(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "Must be >= 0");
            }

            Index = index;
        }

        public int Index { get; }
    }
}