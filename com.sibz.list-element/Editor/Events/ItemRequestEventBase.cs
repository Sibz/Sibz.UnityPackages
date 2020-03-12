using UnityEngine.UIElements;

namespace Sibz.ListElement.Events
{
    public class ItemRequestEventBase<T> : EventBase<T> where T : EventBase<T>, new()
    {
        public int Index;
    }
}