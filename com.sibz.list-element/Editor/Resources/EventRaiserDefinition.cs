﻿using System;
using Sibz.ListElement.UxmlHelpers;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Events
{
    public class EventRaiserDefinition
    {
        private Type eventType;
        private VisualElement target;
        private Action<EventBase> setExtraEventData;

        public VisualElement Control;

        private VisualElement RootElement => Util.GetRootElement(Control);

        private EventRaiserDefinition()
        {
        }

        public void RaiseEvent()
        {
            RootElement.SendEvent(CreateRaiseEvent(eventType, target, setExtraEventData));
        }

        public static EventRaiserDefinition Create<T>(
            VisualElement control,
            Action<EventBase> setExtraEventData = null,
            VisualElement target = null)
            where T : EventBase, new()
        {
            if (control is null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            VisualElement t = target ?? control.GetFirstAncestorOfType<ListElement>();
            if (t is null)
            {
                throw new ArgumentException(
                    $"target must be an element, or control must belong to a ListElement\nControl was: {control + string.Join(" ", control.GetClasses())}",
                    nameof(target));
            }

            return new EventRaiserDefinition
            {
                Control = control,
                target = t,
                eventType = typeof(T),
                setExtraEventData = setExtraEventData
            };
        }

        public static EventBase CreateRaiseEvent(Type eventType, VisualElement target,
            Action<EventBase> setExtraEventData)
        {
            if (eventType is null || target is null)
            {
                throw new ArgumentNullException(eventType is null ? nameof(eventType) : nameof(target));
            }

            if (!eventType.IsSubclassOf(typeof(EventBase)) ||
                !(Activator.CreateInstance(eventType) is EventBase eventBase))
            {
                throw new ArgumentException("eventType must be subclass of EventBase", nameof(eventType));
            }


            eventBase.target = target;
            setExtraEventData?.Invoke(eventBase);
            return eventBase;
        }
    }
}