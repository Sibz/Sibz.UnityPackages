using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Events
{
    public class ListElementEventHandler
    {
        private readonly ListElement listElement;
        private readonly PropertyModificationHandler handler;
        private ButtonBinder[] outerButtonBinders;

        public ListElementEventHandler(ListElement le, PropertyModificationHandler handler)
        {
            listElement = le;
            this.handler = handler;
            RegisterCallbacks();
            CreateOuterButtonBinders();
        }

        [SuppressMessage("ReSharper", "HeapView.DelegateAllocation")]
        private void RegisterCallbacks()
        {
            listElement.RegisterCallback<AddNewRequestedEvent>(OnAddNewRequested);
            listElement.RegisterCallback<ClearListRequestedEvent>(OnClearListRequested);
            listElement.RegisterCallback<ClearListConfirmedEvent>(OnClearListConfirmed);
            listElement.RegisterCallback<ClearListCancelledEvent>(OnClearListCancelled);
            listElement.RegisterCallback<RemoveItemRequestedEvent>(OnRemoveItemRequested);
            listElement.RegisterCallback<MoveItemRequestedEvent>(OnMoveItemRequested);
        }

        private void CreateOuterButtonBinders()
        {
            void RaiseEvent<T>() where T : EventBase, new()
            {
                Debug.Log("Event Raised");
                listElement.SendEvent(new T() {target = listElement});
            }

            outerButtonBinders = new[]
            {
                new ButtonBinder(Constants.AddButtonClassName, RaiseEvent<AddNewRequestedEvent>),
                new ButtonBinder(Constants.DeleteAllButtonClassName, RaiseEvent<ClearListRequestedEvent>),
                new ButtonBinder(Constants.DeleteConfirmButtonClassName, RaiseEvent<ClearListConfirmedEvent>),
                new ButtonBinder(Constants.DeleteCancelButtonClassName, RaiseEvent<ClearListCancelledEvent>),
            };
        }

        public void BindOuterButtons()
        {
            outerButtonBinders.BindButtons(listElement);
        }

        public void BindItemButtons(int index, VisualElement itemSection)
        {
            void RaiseDeleteEvent()
            {
                listElement.SendEvent(new RemoveItemRequestedEvent() {target = listElement, Index = index});
            }

            void RaiseMoveUpEvent()
            {
                listElement.SendEvent(new MoveItemRequestedEvent()
                    {target = listElement, Index = index, Direction = MoveItemRequestedEvent.MoveDirection.Up});
            }

            void RaiseMoveDownEvent()
            {
                listElement.SendEvent(new MoveItemRequestedEvent()
                    {target = listElement, Index = index, Direction = MoveItemRequestedEvent.MoveDirection.Down});
            }

            new ButtonBinder(Constants.DeleteItemButtonClassName, RaiseDeleteEvent).BindToFunction(itemSection);
            new ButtonBinder(Constants.MoveUpButtonClassName, RaiseMoveUpEvent).BindToFunction(itemSection);
            new ButtonBinder(Constants.MoveDownButtonClassName, RaiseMoveDownEvent).BindToFunction(itemSection);
        }

        private void OnAddNewRequested(AddNewRequestedEvent evt)
        {
            handler.Add();
        }

        private void OnClearListRequested(ClearListRequestedEvent evt)
        {
            ToggleConfirmDisplay(true);
        }

        private void OnClearListConfirmed(ClearListConfirmedEvent evt)
        {
            ToggleConfirmDisplay();
            handler.Clear();
        }

        private void OnClearListCancelled(ClearListCancelledEvent evt)
        {
            ToggleConfirmDisplay();
        }

        private void OnRemoveItemRequested(RemoveItemRequestedEvent evt)
        {
            handler.Remove(evt.Index);
        }

        private void OnMoveItemRequested(MoveItemRequestedEvent evt)
        {
            if (evt.Direction == MoveItemRequestedEvent.MoveDirection.Up)
            {
                handler.MoveUp(evt.Index);
            }
            else
            {
                handler.MoveDown(evt.Index);
            }
        }

        private void ToggleConfirmDisplay(bool show = false)
        {
            listElement.Q(null, Constants.DeleteConfirmSectionClassName).style.display =
                show ? DisplayStyle.Flex : DisplayStyle.None;
            listElement.Q(null, Constants.DeleteAllButtonClassName).style.display =
                show ? DisplayStyle.None : DisplayStyle.Flex;
            listElement.Q(null, Constants.AddButtonClassName).style.display =
                show ? DisplayStyle.None : DisplayStyle.Flex;
        }
    }
}