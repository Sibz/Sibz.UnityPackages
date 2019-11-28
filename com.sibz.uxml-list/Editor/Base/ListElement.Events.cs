using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;

namespace Sibz.UXMLList
{
    public class AddActionEvent : EventBase<AddActionEvent>, IListEventWithListProperty
    {
        public SerializedProperty ListProperty { get; set; }
    }

    public class DeleteAllButtonClickedEvent : EventBase<DeleteAllButtonClickedEvent>, IListEventWithListProperty
    {
        public SerializedProperty ListProperty { get; set; }
    }

    public class DeleteAllConfirmedEvent : EventBase<DeleteAllConfirmedEvent>, IListEventWithListProperty
    {
        public SerializedProperty ListProperty { get; set; }
    }

    public class DeleteAllCanceledEvent : EventBase<DeleteAllCanceledEvent>, IListEventWithListProperty
    {
        public SerializedProperty ListProperty { get; set; }
    }

    public class DeleteItemEvent : EventBase<DeleteItemEvent>, IListEventWithListProperty
    {
        public SerializedProperty ListProperty { get; set; }
    }

    public class MoveItemUpEvent : EventBase<MoveItemUpEvent>, IListEventWithListProperty
    {
        public SerializedProperty ListProperty { get; set; }
    }

    public class MoveItemDownEvent : EventBase<MoveItemDownEvent>, IListEventWithListProperty
    {
        public SerializedProperty ListProperty { get; set; }
    }

}