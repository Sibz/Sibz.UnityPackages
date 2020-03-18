using Sibz.ListElement.Internal;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Events
{
    public interface IListElementEventHandler
    {
        PropertyModificationHandler Handler { get; set; }
        void OnAddItem(AddItemEvent evt);
        void OnClearListRequested(ClearListRequestedEvent evt);
        void OnClearList(ClearListEvent evt);
        void OnClearListCancelled(ClearListCancelledEvent evt);
        void OnRemoveItem(RemoveItemEvent evt);
        void OnMoveItem(MoveItemEvent evt);
        void OnClicked(ClickEvent evt);
        void OnReset();
        void OnChanged(ChangeEvent<Object> evt);
        void OnRowInserted(RowInsertedEvent evt);
    }
}