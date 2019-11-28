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

    public class DeleteAllConfirmedAction : EventBase<DeleteAllConfirmedAction>, IListEventWithListProperty
    {
        public SerializedProperty ListProperty { get; set; }
    }

    public class DeleteAllCanceledAction : EventBase<DeleteAllCanceledAction>, IListEventWithListProperty
    {
        public SerializedProperty ListProperty { get; set; }
    }
}