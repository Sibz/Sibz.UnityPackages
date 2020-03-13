using UnityEngine;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Events
{
    public class AddNewRequestedEvent : EventBase<AddNewRequestedEvent>
    {
        public Object Item;
    }
}