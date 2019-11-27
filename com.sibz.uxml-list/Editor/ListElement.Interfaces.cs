using System;
using UnityEditor;
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
    public interface IListElementClickable<T> where T: EventBase
    {
        void OnClicked(T eventData);
    }
    public interface IListElementChangable<TChangeEvent, T> where TChangeEvent: ChangeEvent<T>
    {
        EventCallback<TChangeEvent> ChangedCallback { get; }
    }
	public interface IListEventWithListProperty
    {
        SerializedProperty ListProperty { get; set; }
    }
}
