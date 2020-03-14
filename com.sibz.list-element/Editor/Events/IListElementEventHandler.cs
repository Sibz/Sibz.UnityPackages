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
    }
}