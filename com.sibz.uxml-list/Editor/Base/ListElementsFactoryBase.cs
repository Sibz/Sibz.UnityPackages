using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sibz.UXMLList
{
    /// <summary>
    /// Creates and stores the elements used around the ListVisualElement
    /// </summary>
    public abstract partial class ListElementsFactoryBase
    {
        protected static readonly string CLASS_PREFIX = "sibz-list";

        private readonly ListVisualElement m_Owner;

        private readonly Dictionary<string, VisualElement> m_OutsideElements = new Dictionary<string, VisualElement>();
        private readonly List<VisualElement> m_InsideElements = new List<VisualElement>();
        public bool m_Initialised = false;

        public ListElementsFactoryBase(ListVisualElement owner)
        {
            m_Owner = owner;
            Controls = new ControlsClass(this);
            if (m_Owner is IListControlAccesor)
            {
                m_Owner.Controls = Controls;
            }
        }

        public virtual void Init()
        {
            foreach (var element in m_OutsideElements.Values)
            {
                OnInit_ApplyInterfaces(element);
            }
            foreach (var element in m_InsideElements)
            {
                OnInit_ApplyInterfaces(element);
            }
            m_Initialised = true;
        }

        public virtual void Reset()
        {
            foreach (var element in m_OutsideElements.Values)
            {
                (element as IListElementResetable)?.Reset();
            }
            foreach (var element in m_InsideElements)
            {
                (element as IListElementResetable)?.Reset();
            }
        }

        private TElement CreateElement<TElement>(string name = null) where TElement : VisualElement, new()
        {
            TElement element = new TElement();
            element.AddToClassList($"{CLASS_PREFIX}-{AddSpacesToSentence(name ?? element.GetType().Name).ToLower()}");
            OnCreate_ApplyInterfaces(element);
            if (!string.IsNullOrEmpty(name))
            {
                m_InsideElements.Add(element);
            }
            if (m_Initialised)
            {
                OnInit_ApplyInterfaces(element);
            }

            return element;
        }

        protected T GetOrCreateElement<T>(string name) where T : VisualElement, new()
        {
            if (!m_OutsideElements.ContainsKey(name))
            {
                m_OutsideElements.Add(name, CreateElement<T>());
            }
            return m_OutsideElements[name] as T;
        }

        private void OnCreate_ApplyInterfaces<TElement>(TElement element) where TElement : VisualElement, new()
        {
            if (element is IListElement)
            {
                (element as IListElement).ListElement = m_Owner;
                (element as IListElement).Controls = Controls;
            }

            (element as IListElementInstantiator)?.Instantiate();

            if (ImplementsOpenGenericInterface(element, typeof(IListElementClickable<>)) && TryGetEventType(typeof(IListElementClickable<>), element.GetType(), out Type eventType))
            {
                AddEventHandler(element, nameof(Button.clicked), new Action(() =>
                {
                    var eventInstance = Activator.CreateInstance(eventType) as EventBase;
                    eventInstance.target = element;
                    if (eventInstance is IListEventWithListProperty)
                    {
                        (eventInstance as IListEventWithListProperty).ListProperty = m_Owner.ListProperty;
                    }
                    element.SendEvent(eventInstance);
                }));
                EventRegistration(typeof(IListElementClickable<>).MakeGenericType(eventType), element, eventType);
            }
            if (ImplementsOpenGenericInterface(element, typeof(IListElementChangable<,>)) && TryGetEventType(typeof(IListElementChangable<,>), element.GetType(), out eventType))
            {
                EventRegistration(typeof(IListElementChangable<,>).MakeGenericType(eventType, eventType.GetGenericArguments()[0]), element, eventType);
            }

        }

        private void OnInit_ApplyInterfaces<TElement>(TElement element) where TElement : VisualElement, new()
        {
            (element as IListElementInitialisor)?.Initialise();
        }

        private bool ImplementsOpenGenericInterface(object obj, Type openGenericInterfaceType)
        {
            return obj.GetType().GetInterfaces().Any(iface =>
                    iface.IsGenericType &&
                    iface.GetGenericTypeDefinition().Equals(openGenericInterfaceType));
        }

        private bool TryGetEventType(Type openGenericInterfaceType, Type elementType, out Type eventType)
        {
            eventType = null;

            eventType = elementType.GetInterfaces()
                // Get first nested class with interface IListElementClickedEvent
                .Where(x => x.IsGenericType
                            && x.GetGenericTypeDefinition().Equals(openGenericInterfaceType)
                            && x.GenericTypeArguments.Length > 0
                            )
                .Select(x => x.GenericTypeArguments[0])
                .FirstOrDefault();

            if (!(eventType is Type))
            {
                Debug.LogWarning($"{nameof(ListElementsFactoryBase)}.{nameof(TryGetEventType)}: {openGenericInterfaceType.Name} no valid generic type argument found on {elementType.Name}.");
            }

            return eventType is Type;
        }

        private void AddEventHandler<TElement, TProp>(TElement element, string propName, TProp value) where TElement : VisualElement, new() where TProp : Delegate
        {
            Type elementType = element.GetType();
            if (!elementType.GetEvents().Any(t => t.Name == propName && t.EventHandlerType.Equals(typeof(TProp))))
            {
                Debug.LogWarning($"{nameof(ListElementsFactoryBase)}.{nameof(AddEventHandler)}: unable to find member: {propName} of type {typeof(TProp).Name}");
                return;
            }

            elementType.GetEvent(propName).AddEventHandler(element, value);
        }

        private void EventRegistration<TElement>(Type infaceType, TElement item, Type eventType)
        {
            var registerCallbackMethod = typeof(TElement).GetMethods().Where(m => m.Name == nameof(VisualElement.RegisterCallback)).FirstOrDefault();
            var callbackMethod = infaceType.GetMethods().Where(x => x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType.Equals(eventType)).FirstOrDefault();
            registerCallbackMethod?
                .MakeGenericMethod(eventType)
                .Invoke(item, new object[] { callbackMethod.CreateDelegate(typeof(EventCallback<>).MakeGenericType(eventType), item), null });
        }

        private string AddSpacesToSentence(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "";
            }

            System.Text.StringBuilder newText = new System.Text.StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                {
                    newText.Append('-');
                }

                newText.Append(text[i]);
            }
            return newText.ToString();
        }
    }
}
