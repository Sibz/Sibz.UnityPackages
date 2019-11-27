using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sibz.UXMLList
{
    public interface IListControlAccesor
    {

        ListElementsFactoryBase.ControlsClass Controls { get; set; }
    }
    public interface IListElement : IListControlAccesor
    {
        ListVisualElement ListElement { get; set; }
    }

    public interface IListElementInstantiator : IListElement
    {
        void Instantiate();
    }
    public interface IListElementInitialisor : IListElement
    {
        void Initialise();
    }
    public interface IListElementEventBinder : IListElement
    {
        void BindEventRaiser(Action eventRaiser);
        EventCallback<EventBase> Callback { get; }
    }
    public interface IListElementEventType
    {
        SerializedProperty ListProperty { get; set; }
    }


    /// <summary>
    /// Creates and stores the elements used around the ListVisualElement
    /// </summary>
    public abstract class ListElementsFactoryBase
    {
        protected static readonly string CLASS_PREFIX = "sibz-list";
        private readonly ListVisualElement m_Owner;

        private readonly Dictionary<string, VisualElement> m_OutsideElements = new Dictionary<string, VisualElement>();

        public TElement CreateElement<TElement>() where TElement : VisualElement, new()
        {
            TElement item = new TElement();
            Type type = item.GetType();
            item.AddToClassList($"{CLASS_PREFIX}" + AddSpacesToSentence(type.Name));
            if (item is IListElement)
            {
                (item as IListElement).ListElement = m_Owner;
                (item as IListElement).Controls = Controls;
            }
            if (item is IListElementInstantiator)
            {
                (item as IListElementInstantiator).Instantiate();
            }
            if (TryGetEventType(item, type, out Type eventType))
            {
                BindEventRaiser<TElement>(item as IListElementEventBinder, eventType, type);
                EventRegistration(item as IListElementEventBinder, eventType, type);
            }

            return item;
        }
        private bool TryGetEventType(VisualElement item, Type elementType, out Type eventType)
        {
            eventType = null;
            if (item is IListElementEventBinder)
            {
                eventType = elementType.GetNestedTypes().Where(x => x.GetInterfaces().Any(iface => iface.Name == nameof(IListElementEventType))).FirstOrDefault();
            }

            return eventType is Type;
        }
        private void BindEventRaiser<TElement>(IListElementEventBinder item, Type eventType, Type elementType) where TElement : VisualElement
        {
            item.BindEventRaiser(new Action(() =>
            {
                var eventInstance = Activator.CreateInstance(eventType) as EventBase;
                eventInstance.target = item as TElement;
                (eventInstance as IListElementEventType).ListProperty = m_Owner.ListProperty;
                (item as TElement).SendEvent(eventInstance);
            }));
        }
        private void EventRegistration(IListElementEventBinder item, Type eventType, Type elementType)
        {
            var methods = elementType.GetMethods().Where(m => m.Name == nameof(VisualElement.RegisterCallback)).ToList();
            methods.FirstOrDefault()?.MakeGenericMethod(eventType).Invoke(item, new object[] { (item as IListElementEventBinder).Callback, null });
            //
        }
        public T CreateElement<T>(object userData) where T : VisualElement, new()
        {
            T item = CreateElement<T>();
            item.userData = userData;
            return item;
        }

        public T GetOrCreateElement<T>(string name) where T : VisualElement, new()
        {
            if (!m_OutsideElements.ContainsKey(name))
            {
                m_OutsideElements.Add(name, CreateElement<T>());
            }
            return m_OutsideElements[name] as T;
        }

        //public VisualElement this[Type t]
        //{
        //    get
        //    {
        //        if (!t.IsSubclassOf(typeof(VisualElement)))
        //        {
        //            throw new TypeAccessException("Type of visual element expected");
        //        }

        //        if (!m_OutsideElements.ContainsKey(t))
        //        {
        //            m_OutsideElements.Add(t, Activator.CreateInstance(t) as VisualElement);
        //        }

        //        return m_OutsideElements[t];
        //    }
        //}

        //protected void AddInstantisor<T>(Action<VisualElement, ListVisualElement, ListElementsFactoryBase> instantisor) where T : VisualElement, new()
        //{
        //    if (!(instantisor is Action<VisualElement, ListVisualElement, ListElementsFactoryBase>))
        //    {
        //        return;
        //    }

        //    var t = typeof(T);
        //    if (m_Instantiators.ContainsKey(t))
        //    {
        //        m_Instantiators[t] = instantisor;
        //    }
        //    else
        //    {
        //        m_Instantiators.Add(t, instantisor);
        //    }
        //    if (m_OutsideElements.ContainsKey(t))
        //    {
        //        m_Instantiators[t](m_OutsideElements[t], m_Owner, this);
        //    }
        //}

        //protected void AddInitialisor<T>(Action<ListElementsFactoryBase, ListVisualElement> initiator) where T : VisualElement, new()
        //{
        //    if (!(initiator is Action<ListElementsFactoryBase, ListVisualElement>))
        //    {
        //        return;
        //    }

        //    var t = typeof(T);
        //    if (m_Initialisors.ContainsKey(t))
        //    {
        //        m_Initialisors[t] = initiator;
        //    }
        //    else
        //    {
        //        m_Initialisors.Add(t, initiator);
        //    }
        //}

        #region Controls
        //public class HeaderSection : VisualElement { }
        //public class HeaderLabel : Label { }
        //public class AddButton : Button { }
        //public class DeleteAllButton : Button { }
        //public class DeleteAllConfirmSection : VisualElement { }
        //public class DeleteAllConfirmLabel : Label { }
        //public class DeleteAllYesButton : Button { }
        //public class DeleteAllNoButton : Button { }
        //public class ItemsSection : VisualElement { }

        //public class MoveUpButton : Button { }
        //public class MoveDownButton : Button { }
        //public class DeleteItemButton : Button { }

        public ControlsClass Controls { get; protected set; }
        public class ControlsClass
        {
            protected ListElementsFactoryBase m_Base;
            public VisualElement HeaderSection => GetOrCreateUsingNested<VisualElement>(nameof(HeaderSection));
            public Label HeaderLabel => GetOrCreateUsingNested<Label>(nameof(HeaderLabel));
            public Button AddButton => GetOrCreateUsingNested<Button>(nameof(AddButton));
            public Button DeleteAllButton => GetOrCreateUsingNested<Button>(nameof(DeleteAllButton));
            public VisualElement DeleteAllConfirmSection => GetOrCreateUsingNested<VisualElement>(nameof(DeleteAllConfirmSection));
            public Label DeleteAllConfirmLabel => GetOrCreateUsingNested<Label>(nameof(DeleteAllConfirmLabel));
            public Button DeleteAllYesButton => GetOrCreateUsingNested<Button>(nameof(DeleteAllYesButton));
            public Button DeleteAllNoButton => GetOrCreateUsingNested<Button>(nameof(DeleteAllNoButton));
            public VisualElement ItemsSection => GetOrCreateUsingNested<VisualElement>(nameof(ItemsSection));

            public Button NewMoveUpButton => CreateUsingNested<Button>(nameof(NewMoveUpButton));
            public Button NewMoveDownButton => CreateUsingNested<Button>(nameof(NewMoveDownButton));
            public Button NewDeleteItemButton => CreateUsingNested<Button>(nameof(NewDeleteItemButton));

            public ControlsClass(ListElementsFactoryBase baseFactory)
            {
                m_Base = baseFactory;
            }
            public T GetOrCreateUsingNested<T>(string name) where T : VisualElement, new()
            {
                var t = typeof(T);
                var getOrCreateMethod = m_Base.GetType().GetMethod(nameof(ListElementsFactoryBase.GetOrCreateElement));
                var nestedType = m_Base.GetType().GetNestedTypes().Where(x => x.Name == name && (x.IsSubclassOf(t) || x.IsSubclassOf(t.BaseType))).FirstOrDefault();
                if (nestedType != null)
                {
                    return getOrCreateMethod.MakeGenericMethod(nestedType).Invoke(m_Base, new object[1] { name }) as T;
                }
                return m_Base.GetOrCreateElement<T>(name);
            }
            public T CreateUsingNested<T>(string name) where T : VisualElement, new()
            {
                var t = typeof(T);
                var getOrCreateMethod = m_Base.GetType().GetMethod(nameof(ListElementsFactoryBase.CreateElement));
                var nestedType = m_Base.GetType().GetNestedTypes().Where(x => x.Name == name && (x.IsSubclassOf(t) || x.IsSubclassOf(t.BaseType))).FirstOrDefault();
                if (nestedType != null)
                {
                    return getOrCreateMethod.MakeGenericMethod(nestedType).Invoke(m_Base, new object[0]) as T;
                }
                return m_Base.CreateElement<T>();
            }
        }
        #endregion

        public ListElementsFactoryBase(ListVisualElement owner)
        {
            m_Owner = owner;
            Controls = new ControlsClass(this);
            if (m_Owner is IListControlAccesor)
            {
                m_Owner.Controls = Controls;
            }
        }

        public virtual void Init(ListVisualElement element, Func<SerializedProperty> listPropertyGetter)
        {
            foreach (var item in m_OutsideElements.Values)
            {
                if (item is IListElementInitialisor)
                {
                    (item as IListElementInitialisor).Initialise();
                }
            }
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
