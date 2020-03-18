using System;
using System.Collections.Generic;
using System.Linq;
using Sibz.ListElement.Internal;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Sibz.ListElement.Events
{
    public class ListElementEventHandler : IListElementEventHandler
    {
        private readonly IOuterControls outerControls;
        private readonly IEnumerable<EventRaiserDefinition> outerEventRaisers;
        private readonly EventRaiserDefinition addObjectFieldDropRaiser;
        private readonly List<EventRaiserDefinition> rowEventRaisers = new List<EventRaiserDefinition>();

        public ListElementEventHandler(IOuterControls outerControls)
        {
            outerEventRaisers = CreateRaiserDefinitions(outerControls);
            addObjectFieldDropRaiser = outerEventRaisers.Single(x =>
                x.Control.ClassListContains(UxmlClassNames.AddItemObjectFieldClassName));
            this.outerControls = outerControls;
        }

        public PropertyModificationHandler Handler { get; set; }

        public void OnAddItem(AddItemEvent evt)
        {
            Handler?.Add(evt.Item);
            ElementInteractions.SetAddObjectFieldValueToNull(outerControls.AddObjectField);
        }

        public void OnClearListRequested(ClearListRequestedEvent evt)
        {
            ElementInteractions.SetConfirmSectionVisibility(outerControls.ClearList,
                outerControls.ClearListConfirmSection, true);
        }

        public void OnClearList(ClearListEvent evt)
        {
            ElementInteractions.SetConfirmSectionVisibility(outerControls.ClearList,
                outerControls.ClearListConfirmSection, false);
            Handler?.Clear();
        }

        public void OnClearListCancelled(ClearListCancelledEvent evt)
        {
            ElementInteractions.SetConfirmSectionVisibility(outerControls.ClearList,
                outerControls.ClearListConfirmSection, false);
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

        public void OnClicked(ClickEvent evt)
        {
            RaiseEventBaseOnEvtTarget(evt.target, outerEventRaisers);
            RaiseEventBaseOnEvtTarget(evt.target, rowEventRaisers);
        }

        public void OnChanged(ChangeEvent<Object> evt)
        {
            if (evt.target == addObjectFieldDropRaiser.Control)
            {
                addObjectFieldDropRaiser.RaiseEvent();
            }
        }

        public void OnRowInserted(RowInsertedEvent evt)
        {
            rowEventRaisers.AddRange(CreateRaiserDefinitionsForRow(evt.Buttons, evt.Index));
            ElementInteractions.SetButtonStateBasedOnZeroIndex(evt.Buttons.MoveUp, evt.Index);
            ElementInteractions.SetButtonStateBasedOnBeingLastPositionInArray(evt.Buttons.MoveDown, evt.Index,
                evt.ListLength);
        }

        public void OnListLengthChanged(ChangeEvent<int> evt)
        {
            ListElement le;
            if (evt.target is VisualElement ve && !((le = ve.GetFirstAncestorOfType<ListElement>()) is null))
            {
                le.SendEvent(new ListResetEvent {target = le});
            }
        }

        public void OnReset(ListResetEvent evt)
        {
            rowEventRaisers.Clear();

            if (evt.target is ListElement listElement)
            {
                PopulateList(listElement);
                ElementInteractions.SetButtonStateBasedOnZeroIndex(
                    listElement.Controls.ClearList, listElement.SerializedProperty.arraySize);
            }
        }

        public void OnAttachToPanel(AttachToPanelEvent evt)
        {
            evt.target.SendEvent(new ListResetEvent {target = evt.target});
        }

        public static void PopulateList(ListElement listElement)
        {
            Controls controls = listElement.Controls;
            SerializedProperty property = listElement.SerializedProperty;
            IRowGenerator rowGenerator = listElement.RowGenerator;

            controls.ItemsSection.Clear();

            for (int i = 0; i < property.arraySize; i++)
            {
                controls.ItemsSection.Add(rowGenerator.NewRow(i, property));
                listElement.SendEvent(new RowInsertedEvent
                {
                    target = listElement,
                    Buttons = controls.Row[i],
                    Index = i,
                    ListLength = property.arraySize
                });
            }
        }

        public static IEnumerable<EventRaiserDefinition> CreateRaiserDefinitions(IOuterControls controls)
        {
            void SetExtraEventData(EventBase obj)
            {
                if (!(obj is AddItemEvent evt))
                {
                    throw new ArgumentException($"Expected type: {typeof(AddItemEvent)}");
                }

                evt.Item = controls.AddObjectField.value;
            }

            return new List<EventRaiserDefinition>
            {
                EventRaiserDefinition.Create<ClearListRequestedEvent>(controls.ClearList),
                EventRaiserDefinition.Create<ClearListEvent>(controls.ClearListConfirm),
                EventRaiserDefinition.Create<ClearListCancelledEvent>(controls.ClearListCancel),
                EventRaiserDefinition.Create<AddItemEvent>(controls.Add),
                EventRaiserDefinition.Create<AddItemEvent>(controls.AddObjectField, SetExtraEventData)
            };
        }

        public static IEnumerable<EventRaiserDefinition> CreateRaiserDefinitionsForRow(IRowButtons rowButtons,
            int index)
        {
            void SetMoveUpEventData(EventBase @event)
            {
                SetMoveEventData(@event as MoveItemEvent, MoveItemEvent.MoveDirection.Up);
            }

            void SetMoveDownEventData(EventBase @event)
            {
                SetMoveEventData(@event as MoveItemEvent, MoveItemEvent.MoveDirection.Down);
            }

            void SetRemoveEventData(EventBase @event)
            {
                SetItemEventData<RemoveItemEvent>(@event as RemoveItemEvent);
            }

            void SetMoveEventData(MoveItemEvent @event, MoveItemEvent.MoveDirection direction)
            {
                SetItemEventData<MoveItemEvent>(@event);
                @event.Direction = direction;
            }

            void SetItemEventData<T>(ItemEventBase<T> @event) where T : ItemEventBase<T>, new()
            {
                if (@event is null)
                {
                    throw new ArgumentException(nameof(@event), $"Expected type: {typeof(T)}");
                }

                @event.Index = index;
            }

            return new List<EventRaiserDefinition>
            {
                EventRaiserDefinition.Create<MoveItemEvent>(rowButtons.MoveUp,
                    SetMoveUpEventData),
                EventRaiserDefinition.Create<MoveItemEvent>(rowButtons.MoveDown,
                    SetMoveDownEventData),
                EventRaiserDefinition.Create<RemoveItemEvent>(rowButtons.RemoveItem,
                    SetRemoveEventData)
            };
        }

        // TODO Integration Test
        public static void RaiseEventBaseOnEvtTarget(IEventHandler target,
            IEnumerable<EventRaiserDefinition> eventRaisers)
        {
            var eventRaiserDefinitions = eventRaisers as EventRaiserDefinition[] ?? eventRaisers.ToArray();
            if (target is VisualElement element && eventRaiserDefinitions.Any(x => x.Control == element))
            {
                eventRaiserDefinitions.Single(x => x.Control == element).RaiseEvent();
            }
        }

        public static void RegisterCallbacks(VisualElement element, IListElementEventHandler handler)
        {
            element.RegisterCallback<ClearListRequestedEvent>(handler.OnClearListRequested);
            element.RegisterCallback<ClearListEvent>(handler.OnClearList);
            element.RegisterCallback<ClearListCancelledEvent>(handler.OnClearListCancelled);
            element.RegisterCallback<MoveItemEvent>(handler.OnMoveItem);
            element.RegisterCallback<RemoveItemEvent>(handler.OnRemoveItem);
            element.RegisterCallback<AddItemEvent>(handler.OnAddItem);
            element.RegisterCallback<ClickEvent>(handler.OnClicked);
            element.RegisterCallback<ChangeEvent<Object>>(handler.OnChanged);
            element.RegisterCallback<RowInsertedEvent>(handler.OnRowInserted);
            element.RegisterCallback<ChangeEvent<int>>(handler.OnListLengthChanged);
            element.RegisterCallback<ListResetEvent>(handler.OnReset);
            element.RegisterCallback<AttachToPanelEvent>(handler.OnAttachToPanel);
        }
    }
}