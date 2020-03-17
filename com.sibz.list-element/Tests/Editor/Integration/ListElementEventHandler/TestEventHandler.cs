﻿using System.Collections.Generic;
using Sibz.ListElement.Events;
using Sibz.ListElement.Internal;
using UnityEngine;
using UnityEngine.UIElements;
using Handler = Sibz.ListElement.Internal.PropertyModificationHandler;

namespace Sibz.ListElement.Tests.Integration.ListElementEventHandler
{
    public class TestEventHandler : IListElementEventHandler
    {
        public List<string> EventNames = new List<string>
        {
            nameof(ClearListRequestedEvent),
            nameof(ClearListEvent),
            nameof(ClearListCancelledEvent),
            nameof(MoveItemEvent),
            nameof(RemoveItemEvent),
            nameof(AddItemEvent),
            nameof(ClickEvent),
            typeof(ChangeEvent<Object>).Name
        };

        public Handler Handler { get; set; }

        public void OnAddItem(AddItemEvent evt)
        {
            EventNames.Remove(evt.GetType().Name);
        }

        public void OnClearListRequested(ClearListRequestedEvent evt)
        {
            EventNames.Remove(evt.GetType().Name);
        }

        public void OnClearList(ClearListEvent evt)
        {
            EventNames.Remove(evt.GetType().Name);
        }

        public void OnClearListCancelled(ClearListCancelledEvent evt)
        {
            EventNames.Remove(evt.GetType().Name);
        }

        public void OnRemoveItem(RemoveItemEvent evt)
        {
            EventNames.Remove(evt.GetType().Name);
        }

        public void OnMoveItem(MoveItemEvent evt)
        {
            EventNames.Remove(evt.GetType().Name);
        }

        public void OnClicked(ClickEvent evt)
        {
            EventNames.Remove(evt.GetType().Name);
        }

        public void OnReset()
        {
            throw new System.NotImplementedException();
        }

        public void OnAddRow(IRowButtons buttons, int index)
        {
            throw new System.NotImplementedException();
        }

        public void OnChanged(ChangeEvent<Object> evt)
        {
            EventNames.Remove(evt.GetType().Name);
        }
    }
}