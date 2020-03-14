using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Events
{
    public class ListElementEventHandler : IListElementEventHandler
    {
        private readonly ListElement listElement;

        private ButtonBinder[] outerButtonBinders;

        public ListElementEventHandler(ListElement le)
        {
            listElement = le;
        }

        public PropertyModificationHandler Handler { get; set; }

        public void OnAddItem(AddItemEvent evt)
        {
            Handler?.Add(evt.Item);
            ResetAddObjectFieldValueToNull();
        }

        public void OnClearListRequested(ClearListRequestedEvent evt)
        {
            ToggleConfirmDisplay(true);
        }

        public void OnClearList(ClearListEvent evt)
        {
            ToggleConfirmDisplay();
            Handler?.Clear();
        }

        public void OnClearListCancelled(ClearListCancelledEvent evt)
        {
            ToggleConfirmDisplay();
        }

        public void OnRemoveItem(RemoveItemEvent evt)
        {
            Handler?.Remove(evt.Index);
        }

        public void OnMoveItem(MoveItemEvent evt)
        {
            if (evt.Direction == MoveItemEvent.MoveDirection.Up)
            {
                Handler?.MoveUp(evt.Index);
            }
            else
            {
                Handler?.MoveDown(evt.Index);
            }
        }

        private void ResetAddObjectFieldValueToNull()
        {
            listElement.Q<ObjectField>(null, Constants.AddItemObjectField).SetValueWithoutNotify(null);
        }

        private void ToggleConfirmDisplay(bool show = false)
        {
            listElement.Q(null, Constants.DeleteConfirmSectionClassName).style.display =
                show ? DisplayStyle.Flex : DisplayStyle.None;
            listElement.Q(null, Constants.DeleteAllButtonClassName).style.display =
                show ? DisplayStyle.None : DisplayStyle.Flex;
        }
    }
}