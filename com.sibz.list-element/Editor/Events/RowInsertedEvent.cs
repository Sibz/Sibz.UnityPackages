using UnityEngine.UIElements;

namespace Sibz.ListElement.Events
{
    public class RowInsertedEvent : EventBase<RowInsertedEvent>
    {
        public IRowButtons Buttons;
        public int Index;
        public int ListLength;
    }
}