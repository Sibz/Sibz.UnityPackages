﻿using System;
using UnityEditor;
using UnityEditor.UIElements;
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

        public class AddButton : Button, IListElementInitialisor, IListElementClickable<AddActionEvent>
        {
            public ListVisualElement ListElement { get; set; }
            public ControlsClass Controls { get; set; }

            public void Initialise()
            {
                text = ListElement.AddButtonText;
                style.display = ListElement.HideAddButton ? DisplayStyle.None : DisplayStyle.Flex;
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

        public class DeleteAllButton : Button, IListElementInitialisor, IListElementClickable<DeleteAllButtonClickedEvent>, IListElementResetable
        {
            public ListVisualElement ListElement { get; set; }
            public ControlsClass Controls { get; set; }

            public void Initialise()
            {
                text = ListElement.DeleteAllButtonText;
                style.display = ListElement.HideDeleteAllButton ? DisplayStyle.None : DisplayStyle.Flex;
            }

            public void OnClicked(DeleteAllButtonClickedEvent eventData)
            {
                Controls.HeaderSection.style.display = DisplayStyle.None;
                Controls.DeleteAllConfirmSection.style.display = DisplayStyle.Flex;
            }

            public void Reset()
            {
                SetEnabled(ListElement.ListProperty.arraySize > 0);
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

        public class DeleteAllYesButton : Button, IListElementInitialisor, IListElementClickable<DeleteAllConfirmedAction>
        {
            public ListVisualElement ListElement { get; set; }
            public ControlsClass Controls { get; set; }

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

        public class DeleteAllNoButton : Button, IListElementInitialisor, IListElementClickable<DeleteAllCanceledAction>
        {

            public ListVisualElement ListElement { get; set; }
            public ControlsClass Controls { get; set; }

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

        public class ItemsSection : VisualElement, IListElementResetable
        {
            public ListVisualElement ListElement { get; set; }
            public ControlsClass Controls { get; set; }

            public void Reset()
            {
                var property = ListElement.ListProperty;
                Clear();
                if (property.isArray)
                {
                    Controls.DeleteAllButton.SetEnabled(property.arraySize > 0);
                    var endProperty = property.GetEndProperty();

                    property.NextVisible(true);
                    do
                    {

                        if (SerializedProperty.EqualContents(property, endProperty))
                        {
                            break;
                        }

                        switch (property.propertyType)
                        {
                            case SerializedPropertyType.ArraySize:
                                var field = new IntegerField { bindingPath = property.propertyPath };
                                field.SetValueWithoutNotify(property.intValue); // This avoids the OnValueChanged/Rebind feedback loop.
                                field.style.display = ListElement.ShowSize ? DisplayStyle.Flex : DisplayStyle.None;
                                field.RegisterValueChangedCallback(UpdateList);
                                field.label = "Size";
                                Add(field);
                                break;

                            default:
                                var f = Controls.ItemPropertyField;
                                //f.bindingPath = property.propertyPath;
                                f.BindProperty(property);
                                Add(f);
                                if (ListElement.DisablePropertyLabel)
                                {
                                    f.RegisterCallback<AttachToPanelEvent>((e) =>
                                    {

                                        if (f.Q<Label>() is Label)
                                        {
                                            f.Q<Label>().style.display = DisplayStyle.None;
                                        }
                                    });
                                }

                                if (!ListElement.DisablePropertyLabel && ListElement.DisableLabelContextMenu)
                                {
                                    f.RegisterCallback<MouseUpEvent>((e) =>
                                    {
                                        if (e.target is Label && ((Label)e.target).parent?.parent == f)
                                        {
                                            e.StopPropagation();
                                        }
                                    }, TrickleDown.TrickleDown);
                                }

                                break;
                        }

                    } while (property.NextVisible(false));

                    property.Reset();

                }
                else
                {
                    Add(new Label("Error, Bound item is not a list or array"));
                }
            }

            private void UpdateList(ChangeEvent<int> changeEvent)
            {
                ListElement.Unbind();
                ListElement.ListProperty.serializedObject.UpdateIfRequiredOrScript();
                ListElement.ListProperty.serializedObject.ApplyModifiedProperties();
                ListElement.Bind(ListElement.ListProperty.serializedObject);

                changeEvent.StopImmediatePropagation();
            }
        }
    }

}
