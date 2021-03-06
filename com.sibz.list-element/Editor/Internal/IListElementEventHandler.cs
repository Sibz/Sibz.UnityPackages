﻿using Sibz.ListElement.Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Internal
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
        void OnChanged(ChangeEvent<Object> evt);
        void OnRowInserted(RowInsertedEvent evt);
        void OnListLengthChanged(ChangeEvent<int> evt);
        void OnReset(ListResetEvent evt);
        void OnAttachToPanel(AttachToPanelEvent evt);
    }
}