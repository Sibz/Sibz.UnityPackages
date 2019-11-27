using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sibz.UXMLList
{
    public class ListElementsFactory : ListElementsFactoryBase
    {
        public ListElementsFactory(ListVisualElement owner) : base(owner) { }

        public class HeaderSection : VisualElement, IListElementInstantiator
        {
            public ListVisualElement ListElement { get; set; }
            public ControlsClass Controls { get; set; }

            public void Instantiate()
            {
                style.flexDirection = FlexDirection.Row;
                Add(Controls.HeaderLabel);
                Add(Controls.DeleteAllButton);
                Add(Controls.AddButton);
            }
        }

        public class HeaderLabel : Label, IListElementInstantiator, IListElementInitialisor
        {
            public ListVisualElement ListElement { get; set; }
            public ControlsClass Controls { get; set; }

            public void Instantiate()
            {
                style.flexGrow = 1;
                style.unityTextAlign = TextAnchor.MiddleLeft;
            }

            public void Initialise()
            {
                Controls.HeaderLabel.text = ListElement.Label;
                Controls.HeaderLabel.style.visibility = string.IsNullOrEmpty(ListElement.Label) ? Visibility.Hidden : Visibility.Visible;
            }
        }

        public class AddButton : Button, IListElementInitialisor, IListElementClickable<AddButton.AddActionEvent>
        {
            public ListVisualElement ListElement { get; set; }
            public ControlsClass Controls { get; set; }

            public class AddActionEvent : EventBase<AddActionEvent>, IListEventWithListProperty
            {
                public SerializedProperty ListProperty { get; set; }
            }

            public void Initialise()
            {
                text = ListElement.AddButtonText;
                style.display = ListElement.ShowAddButton ? DisplayStyle.Flex : DisplayStyle.None;
            }

            public void OnClicked(AddActionEvent eventData)
            {
                var listProperty = eventData.ListProperty;
                if (listProperty.isArray)
                {
                    listProperty.InsertArrayElementAtIndex(listProperty.arraySize);
                    listProperty.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        public class DeleteAllButton : Button, IListElementInitialisor, IListElementClickable<DeleteAllButton.DeleteAllButtonClickedEvent>
        {
            public ListVisualElement ListElement { get; set; }
            public ControlsClass Controls { get; set; }

            public EventCallback<EventBase> ClickedCallback => (e) =>
            {

            };
            public void BindEventRaiser(Action eventRaiser) { clicked += eventRaiser; }

            public class DeleteAllButtonClickedEvent : EventBase<DeleteAllButtonClickedEvent>, IListEventWithListProperty
            {
                public SerializedProperty ListProperty { get; set; }
            }

            public void Initialise()
            {
                text = ListElement.DeleteAllButtonText;
            }

            public void OnClicked(DeleteAllButtonClickedEvent eventData)
            {
                Controls.HeaderSection.style.display = DisplayStyle.None;
                Controls.DeleteAllConfirmSection.style.display = DisplayStyle.Flex;
            }
        }

        public class DeleteAllConfirmSection : VisualElement, IListElementInstantiator
        {
            public ListVisualElement ListElement { get; set; }
            public ControlsClass Controls { get; set; }

            public void Instantiate()
            {
                style.flexDirection = FlexDirection.Row;
                style.display = DisplayStyle.None;
                Add(Controls.DeleteAllConfirmLabel);
                Add(Controls.DeleteAllYesButton);
                Add(Controls.DeleteAllNoButton);
            }
        }

        public class DeleteAllConfirmLabel : Label, IListElementInstantiator, IListElementInitialisor
        {

            public ListVisualElement ListElement { get; set; }
            public ControlsClass Controls { get; set; }

            public void Initialise()
            {
                text = "Are you sure?";
            }

            public void Instantiate()
            {
                style.unityTextAlign = TextAnchor.MiddleRight;
                style.flexGrow = 1;
            }
        }

        public class DeleteAllYesButton : Button, IListElementInitialisor, IListElementClickable<DeleteAllYesButton.DeleteAllConfirmedAction>
        {
            public ListVisualElement ListElement { get; set; }
            public ControlsClass Controls { get; set; }

            public class DeleteAllConfirmedAction : EventBase<DeleteAllConfirmedAction>, IListEventWithListProperty
            {
                public SerializedProperty ListProperty { get; set; }
            }

            public void Initialise()
            {
                text = "Yes";
            }

            public void OnClicked(DeleteAllConfirmedAction eventData)
            {
                eventData.ListProperty.ClearArray();
                eventData.ListProperty.serializedObject.ApplyModifiedProperties();
                Controls.HeaderSection.style.display = DisplayStyle.Flex;
                Controls.DeleteAllConfirmSection.style.display = DisplayStyle.None;
            }
        }

        public class DeleteAllNoButton : Button, IListElementInitialisor, IListElementClickable<DeleteAllNoButton.DeleteAllCanceledAction>
        {

            public ListVisualElement ListElement { get; set; }
            public ControlsClass Controls { get; set; }

            public class DeleteAllCanceledAction : EventBase<DeleteAllCanceledAction>, IListEventWithListProperty
            {
                public SerializedProperty ListProperty { get; set; }
            }

            public void Initialise()
            {
                text = "No";
            }

            public void OnClicked(DeleteAllCanceledAction eventData)
            {
                Controls.HeaderSection.style.display = DisplayStyle.Flex;
                Controls.DeleteAllConfirmSection.style.display = DisplayStyle.None;
            }
        }
    }

}
