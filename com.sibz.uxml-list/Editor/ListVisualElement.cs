using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sibz.UXMLList
{
    public class ListVisualElement : BindableElement, IListControlAccesor
    {
        public static readonly string UssClassName = "sibz-list";

        public ListElementsFactoryBase.ControlsClass Controls { get; set; }

        #region Public Properties
        public string Label { get; set; }
        public bool ShowSize { get; set; }
        public bool DisableLabelContextMenu { get; set; }
        public bool DisablePropertyLabel { get; set; }
        public bool ShowAddButton { get; set; } = true;
        public string AddButtonText { get; set; } = "+";
        public bool ShowDeleteAllButton { get; set; } = true;
        public string DeleteAllButtonText { get; set; } = "Clear List";

        public override VisualElement contentContainer => m_ListContentContainer ?? base.contentContainer;
        #endregion


        protected VisualElement m_ListContentContainer;
        protected SerializedObject m_SerializedObject;
        public SerializedProperty ListProperty => m_SerializedObject.FindProperty(m_ListPropertyBindingPath);
        protected System.Func<SerializedProperty> ListPropertyGetter => () => ListProperty;
        protected readonly ListElementsFactory m_ListElementsFactory;

        private string m_ListPropertyBindingPath;

        protected class ListElementsFactory : ListElementsFactoryBase
        {
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
            public class AddButton : Button, IListElementInitialisor, IListElementEventBinder
            {
                public ListVisualElement ListElement { get; set; }
                public ControlsClass Controls { get; set; }

                public void BindEventRaiser(Action eventRaiser) { clicked += eventRaiser; }
                public EventCallback<EventBase> Callback => (e) =>
                {
                    var listProperty = (e as AddActionEvent).ListProperty;
                    if (listProperty.isArray)
                    {
                        listProperty.InsertArrayElementAtIndex(listProperty.arraySize);
                        listProperty.serializedObject.ApplyModifiedProperties();
                    }
                };

                public class AddActionEvent : EventBase<AddActionEvent>, IListElementEventType
                {
                    public SerializedProperty ListProperty { get; set; }
                }

                public void Initialise()
                {
                    text = ListElement.AddButtonText;
                    style.display = ListElement.ShowAddButton ? DisplayStyle.Flex : DisplayStyle.None;
                }

            }

            public class DeleteAllButton : Button, IListElementInitialisor, IListElementEventBinder
            {
                public ListVisualElement ListElement { get; set; }
                public ControlsClass Controls { get; set; }

                public EventCallback<EventBase> Callback => (e) =>
                {
                    Controls.HeaderSection.style.display = DisplayStyle.None;
                    Controls.DeleteAllConfirmSection.style.display = DisplayStyle.Flex;
                };
                public void BindEventRaiser(Action eventRaiser) { clicked += eventRaiser; }

                public class DeleteAllButtonClickedEvent : EventBase<DeleteAllButtonClickedEvent>, IListElementEventType
                {
                    public SerializedProperty ListProperty { get; set; }
                }

                public void Initialise()
                {
                    text = ListElement.DeleteAllButtonText;
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

            public class DeleteAllYesButton : Button, IListElementInitialisor, IListElementEventBinder
            {

                public ListVisualElement ListElement { get; set; }
                public ControlsClass Controls { get; set; }

                public EventCallback<EventBase> Callback => (e) =>
                {
                    var eventAction = (e as DeleteAllConfirmedAction);
                    eventAction.ListProperty.ClearArray();
                    eventAction.ListProperty.serializedObject.ApplyModifiedProperties();
                    Controls.HeaderSection.style.display = DisplayStyle.Flex;
                    Controls.DeleteAllConfirmSection.style.display = DisplayStyle.None;
                };

                public void BindEventRaiser(Action eventRaiser) { clicked += eventRaiser; }

                public class DeleteAllConfirmedAction : EventBase<DeleteAllConfirmedAction>, IListElementEventType
                {
                    public SerializedProperty ListProperty { get; set; }
                }


                public void Initialise()
                {
                    text = "Yes";
                }
            }
            public class DeleteAllNoButton : Button, IListElementInitialisor, IListElementEventBinder
            {

                public ListVisualElement ListElement { get; set; }
                public ControlsClass Controls { get; set; }

                public void Initialise()
                {
                    text = "No";
                }
                public EventCallback<EventBase> Callback => (e) =>
                {
                    var eventAction = (e as DeleteAllCanceledAction);
                    Controls.HeaderSection.style.display = DisplayStyle.Flex;
                    Controls.DeleteAllConfirmSection.style.display = DisplayStyle.None;
                };

                public void BindEventRaiser(Action eventRaiser) { clicked += eventRaiser; }

                public class DeleteAllCanceledAction : EventBase<DeleteAllCanceledAction>, IListElementEventType
                {
                    public SerializedProperty ListProperty { get; set; }
                }
            }
            //protected class HeaderLabelCustomistion : ListElementCustomisation<HeaderLabel>
            //{
            //    public HeaderLabelCustomistion()
            //    {
            //        Instantisor = (label, list, f) =>
            //        {
            //            label.style.flexGrow = 1;
            //            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            //        };
            //    }
            //}

            //protected class AddButtonCustomisation : ListElementCustomisationWithCallback<AddButton, HeaderActionEvent>
            //{
            //    public AddButtonCustomisation()
            //    {
            //        Initialisor = (button, listPropertyGetter, listElement, f) =>
            //        {
            //            f.Controls.AddButton.text = listElement.AddButtonText;
            //            f.Controls.AddButton.style.display = listElement.ShowAddButton ? DisplayStyle.Flex : DisplayStyle.None;

            //            f.Controls.AddButton.clicked += () =>
            //            {
            //                button.SendEvent(new HeaderActionEvent { target = button, ListProperty = listPropertyGetter(), Type = HeaderActionEvent.ActionType.Add });
            //            };
            //        };
            //        Callback = (e) =>
            //        {
            //            if (e.ListProperty.isArray)
            //            {
            //                e.ListProperty.InsertArrayElementAtIndex(e.ListProperty.arraySize);
            //                e.ListProperty.serializedObject.ApplyModifiedProperties();
            //            }
            //        };
            //    }
            //}

            //protected class HeaderSectionCustomisation : ListElementCustomisation<HeaderSection>
            //{
            //    public HeaderSectionCustomisation()
            //    {
            //        Instantisor = (headerSection, list, f) =>
            //        {
            //            headerSection.style.flexDirection = FlexDirection.Row;
            //            headerSection.Add(f.Controls.HeaderLabel);
            //            headerSection.Add(f.Controls.DeleteAllButton);
            //            headerSection.Add((f.Controls as ControlsClass).AddButton);
            //        };
            //    }
            //}

            //protected class DeleteAllConfirmLabelCustomisation : ListElementCustomisation<DeleteAllConfirmLabel>
            //{
            //    public DeleteAllConfirmLabelCustomisation()
            //    {
            //        Instantisor = (label, list, f) =>
            //        {
            //            label.style.unityTextAlign = TextAnchor.MiddleRight;
            //            label.style.flexGrow = 1;
            //        };
            //    }
            //}

            //protected class DeleteAllConfirmSectionCustomisation : ListElementCustomisation<DeleteAllConfirmSection>
            //{
            //    public DeleteAllConfirmSectionCustomisation()
            //    {
            //        Instantisor = (section, list, f) =>
            //        {
            //            section.style.flexDirection = FlexDirection.Row;
            //            section.Add(f.Controls.DeleteAllConfirmLabel);
            //            section.Add(f.Controls.DeleteAllYesButton);
            //            section.Add(f.Controls.DeleteAllNoButton);
            //        };
            //    }

            //}

            //protected class ItemsSectionCustomisation : ListElementCustomisation<ItemsSection>
            //{
            //    public ItemsSectionCustomisation()
            //    {
            //        Instantisor = (section, list, f) =>
            //        {
            //            section.name = "list-contents";
            //            section.Add(new VisualElement());
            //        };
            //    }
            //}

            public ListElementsFactory(ListVisualElement owner) : base(owner)
            {
                //Controls = new ControlsClass(this);
            }

            public override void Init(ListVisualElement element, System.Func<SerializedProperty> listPropertyGetter)
            {
                base.Init(element, listPropertyGetter);

                //Controls.AddButton.text = element.AddButtonText;
                //Controls.AddButton.style.display = element.ShowAddButton ? DisplayStyle.Flex : DisplayStyle.None;

                //Controls.DeleteAllButton.text = element.DeleteAllButtonText;
                //Controls.DeleteAllButton.style.display = element.ShowDeleteAllButton ? DisplayStyle.Flex : DisplayStyle.None;

                //Controls.HeaderLabel.text = element.Label;
                //Controls.HeaderLabel.style.visibility = string.IsNullOrEmpty(element.Label) ? Visibility.Hidden : Visibility.Visible;

                //Controls.HeaderSection.style.display =
                //     // Controls.AddButton.style.display == DisplayStyle.Flex ||
                //     Controls.DeleteAllButton.style.display == DisplayStyle.Flex ||
                //     Controls.HeaderLabel.style.visibility == Visibility.Visible
                //     ? DisplayStyle.Flex : DisplayStyle.None;

                //Controls.DeleteAllYesButton.text = "Yes";
                //Controls.DeleteAllNoButton.text = "No";

                //GetOrCreateElement<DeleteAllConfirmSection>().style.display = DisplayStyle.None;
            }


        }

        public new class UxmlFactory : UxmlFactory<ListVisualElement, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_PropertyPath;
            UxmlStringAttributeDescription m_Label;

            UxmlBoolAttributeDescription m_DisableLabelContextMenu;
            UxmlBoolAttributeDescription m_DisablePropertyLabel;

            UxmlBoolAttributeDescription m_ShowAddButton;
            UxmlStringAttributeDescription m_AddButtonText;

            UxmlBoolAttributeDescription m_ShowDeleteAllButton;
            UxmlStringAttributeDescription m_DeleteAllButtonText;

            //UxmlBoolAttributeDescription m_ShowSize;

            public UxmlTraits()
            {
                m_PropertyPath = new UxmlStringAttributeDescription { name = "binding-path" };
                m_Label = new UxmlStringAttributeDescription { name = "label" };
                m_DisableLabelContextMenu = new UxmlBoolAttributeDescription { name = "disable-label-context-menu" };
                m_DisablePropertyLabel = new UxmlBoolAttributeDescription { name = "disable-property-label" };
                m_ShowAddButton = new UxmlBoolAttributeDescription { name = "show-add-button", defaultValue = true };
                m_AddButtonText = new UxmlStringAttributeDescription { name = "add-button-text", defaultValue = "+" };
                m_ShowDeleteAllButton = new UxmlBoolAttributeDescription { name = "show-deleteAll-button", defaultValue = true };
                m_DeleteAllButtonText = new UxmlStringAttributeDescription { name = "deleteall-button-text", defaultValue = "Clear List" };
                //m_ShowSize = new UxmlBoolAttributeDescription { name = "show-size" };
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                if (!(ve is ListVisualElement field))
                {
                    return;
                }

                string propPath = m_PropertyPath.GetValueFromBag(bag, cc);

                if (!string.IsNullOrEmpty(propPath))
                {
                    field.bindingPath = propPath;
                }

                field.DisableLabelContextMenu = m_DisableLabelContextMenu.GetValueFromBag(bag, cc);
                field.DisablePropertyLabel = m_DisablePropertyLabel.GetValueFromBag(bag, cc);
                field.Label = m_Label.GetValueFromBag(bag, cc);

                field.ShowAddButton = m_ShowAddButton.GetValueFromBag(bag, cc);
                field.AddButtonText = m_AddButtonText.GetValueFromBag(bag, cc);

                field.ShowDeleteAllButton = m_ShowDeleteAllButton.GetValueFromBag(bag, cc);
                field.DeleteAllButtonText = m_DeleteAllButtonText.GetValueFromBag(bag, cc);

                field.m_ListElementsFactory.Init(field, field.ListPropertyGetter);
            }
        }

        public ListVisualElement() : this(null, string.Empty) { }
        public ListVisualElement(SerializedProperty property) : this(property, string.Empty) { }
        public ListVisualElement(SerializedProperty property, string label)
        {

            m_ListElementsFactory = new ListElementsFactory(this);
            AddToClassList(UssClassName);

            Add(m_ListElementsFactory.Controls.HeaderSection);

            Add(m_ListElementsFactory.Controls.DeleteAllConfirmSection);

            Add(m_ListElementsFactory.Controls.ItemsSection);

            m_ListContentContainer = m_ListElementsFactory.Controls.ItemsSection;

            ////RegisterOutsideEvents();

            //Label = label;

            //if (property == null)
            //{
            //    return;
            //}

            //m_SerializedObject = property.serializedObject;

            //bindingPath = property.propertyPath;
        }

        //private void CreateContentContainer()
        //{
        //    VisualElement listContents = new VisualElement { name = "list-content" };
        //    listContents.AddToClassList("sibz-list-container");
        //    Add(listContents);
        //    m_ListContentContainer = listContents;
        //}

        #region Event Registration &  Handlers
        private void RegisterOutsideEvents()
        {
            //          m_ListElementsFactory.AddNewItemButton.RegisterCallback<MouseUpEvent>(AddNewItemHandler);
            //m_ListElementsFactory.Controls.AddButton.clicked += () =>
            //{
            //    SendEvent(new AddButtonClickEvent { target = m_ListElementsFactory.Controls.AddButton, ListProperty = ListProperty });
            //};
            //m_ListElementsFactory.Controls.AddButton.RegisterCallback<AddButtonClickEvent>(AddButtonClickHandler);


            m_ListElementsFactory.Controls.DeleteAllButton.clicked += () =>
                SendEvent(new DeleteAllButtonClickEvent
                {
                    target = m_ListElementsFactory.Controls.DeleteAllButton,
                    Element = this,
                    ListProperty = ListProperty,
                    Button = DeleteAllButtonClickEvent.ButtonType.InitialClick
                });
            m_ListElementsFactory.Controls.DeleteAllYesButton.clicked += () =>
                SendEvent(new DeleteAllButtonClickEvent
                {
                    target = m_ListElementsFactory.Controls.DeleteAllButton,
                    Element = this,
                    ListProperty = ListProperty,
                    Button = DeleteAllButtonClickEvent.ButtonType.Yes
                });
            m_ListElementsFactory.Controls.DeleteAllNoButton.clicked += () =>
                SendEvent(new DeleteAllButtonClickEvent
                {
                    target = m_ListElementsFactory.Controls.DeleteAllButton,
                    Element = this,
                    ListProperty = ListProperty,
                    Button = DeleteAllButtonClickEvent.ButtonType.No
                });

            m_ListElementsFactory.Controls.DeleteAllButton.RegisterCallback<DeleteAllButtonClickEvent>(DeleteAllButtonClickHandler);
            m_ListElementsFactory.Controls.DeleteAllYesButton.RegisterCallback<DeleteAllButtonClickEvent>(DeleteAllButtonClickHandler);
            m_ListElementsFactory.Controls.DeleteAllNoButton.RegisterCallback<DeleteAllButtonClickEvent>(DeleteAllButtonClickHandler);

        }

        private void RegisterItemEvents(SerializedProperty itemProperty)
        {

        }
        protected virtual void AddButtonClickHandler(AddButtonClickEvent e)
        {
            if (e.ListProperty.isArray)
            {
                e.ListProperty.InsertArrayElementAtIndex(e.ListProperty.arraySize);
                e.ListProperty.serializedObject.ApplyModifiedProperties();
            }
        }

        //public class ListItemActionEvent : EventBase<ListItemActionEvent>
        //{
        //    public ListElementsFactoryBase.ListElementType Action { get; set; }
        //    public SerializedProperty ListProperty { get; set; }
        //    public SerializedProperty ItemProperty { get; set; }
        //    public int Index { get; set; }

        //}

        public class AddButtonClickEvent : EventBase<AddButtonClickEvent>
        {
            public SerializedProperty ListProperty { get; set; }
        }

        protected virtual void DeleteAllButtonClickHandler(DeleteAllButtonClickEvent e)
        {
            switch (e.Button)
            {
                case DeleteAllButtonClickEvent.ButtonType.InitialClick:
                    m_ListElementsFactory.Controls.HeaderSection.style.display = DisplayStyle.None;
                    m_ListElementsFactory.Controls.DeleteAllConfirmSection.style.display = DisplayStyle.Flex;
                    break;
                case DeleteAllButtonClickEvent.ButtonType.Yes:
                    e.ListProperty.ClearArray();
                    e.ListProperty.serializedObject.ApplyModifiedProperties();
                    goto case DeleteAllButtonClickEvent.ButtonType.No;
                case DeleteAllButtonClickEvent.ButtonType.No:
                default:
                    m_ListElementsFactory.Controls.HeaderSection.style.display = DisplayStyle.Flex;
                    m_ListElementsFactory.Controls.DeleteAllConfirmSection.style.display = DisplayStyle.None;
                    break;
            }
        }

        public class DeleteAllButtonClickEvent : EventBase<DeleteAllButtonClickEvent>
        {
            public ListVisualElement Element { get; set; }
            public SerializedProperty ListProperty { get; set; }
            public ButtonType Button { get; set; }

            public enum ButtonType
            {
                InitialClick,
                Yes,
                No
            }
        }
        #endregion

        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultActionAtTarget(evt);
            System.Type type = evt.GetType();
            if (type.Name == "SerializedPropertyBindEvent"
                &&
                type.GetProperty("bindProperty").GetValue(evt) is SerializedProperty property)
            {
                Reset(property);

                m_SerializedObject = property.serializedObject;
                m_ListPropertyBindingPath = ((ListVisualElement)evt.target).bindingPath;

                // Don't allow the binding of `this` to continue because `this` is not
                // the actually bound field, it is just a container.
                evt.StopPropagation();
            }
        }

        private void Reset(SerializedProperty prop)
        {

            m_ListContentContainer.Clear();
            if (prop.isArray)
            {
                m_ListElementsFactory.Controls.DeleteAllButton.SetEnabled(prop.arraySize > 0);
                var endProperty = prop.GetEndProperty();

                prop.NextVisible(true);
                do
                {

                    if (SerializedProperty.EqualContents(prop, endProperty))
                    {
                        break;
                    }

                    switch (prop.propertyType)
                    {
                        case SerializedPropertyType.ArraySize:
                            var field = new IntegerField { bindingPath = prop.propertyPath };
                            field.SetValueWithoutNotify(prop.intValue); // This avoids the OnValueChanged/Rebind feedback loop.
                            field.style.display = ShowSize ? DisplayStyle.Flex : DisplayStyle.None;
                            field.RegisterValueChangedCallback(UpdateList);
                            field.label = "Size";
                            m_ListContentContainer.Add(field);
                            break;

                        default:
                            var f = new PropertyField(prop);
                            m_ListContentContainer.Add(f);
                            if (DisablePropertyLabel)
                            {
                                f.RegisterCallback<AttachToPanelEvent>((e) =>
                                    {

                                        if (f.Q<Label>() is Label)
                                        {
                                            f.Q<Label>().style.display = DisplayStyle.None;
                                        }
                                        //Debug.Log(f.childCount);
                                    });
                            }

                            if (!DisablePropertyLabel && DisableLabelContextMenu)
                            {
                                f.RegisterCallback<MouseUpEvent>((e) =>
                                    {
                                        //Debug.Log(((Label)e.target).parent?.parent.GetType());
                                        if (e.target is Label && ((Label)e.target).parent?.parent == f)
                                        {
                                            e.StopPropagation();
                                        }
                                    }, TrickleDown.TrickleDown);
                            }

                            break;
                    }

                } while (prop.NextVisible(false));

                prop.Reset();

            }
            else
            {
                m_ListContentContainer.Add(new Label("Error, Bound item is not a list or array"));
            }
        }

        private void UpdateList(ChangeEvent<int> changeEvent)
        {
            this.Unbind();
            m_SerializedObject.UpdateIfRequiredOrScript();
            m_SerializedObject.ApplyModifiedProperties();
            this.Bind(m_SerializedObject);

            // Enable/Disable Delete All Button
            m_ListElementsFactory.Controls.DeleteAllButton.SetEnabled(ListProperty.arraySize > 0);
            changeEvent.StopImmediatePropagation();
        }
    }
}