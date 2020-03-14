using UnityEngine.UIElements;

namespace Sibz.ListElement.Events
{
    public class ItemEventBase<T> : EventBase<T> where T : EventBase<T>, new()
    {
        public int Index;
    }
}