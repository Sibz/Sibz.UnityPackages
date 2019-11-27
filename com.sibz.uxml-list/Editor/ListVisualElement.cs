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

        public SerializedProperty ListProperty => m_SerializedObject.FindProperty(m_ListPropertyBindingPath);

        public override VisualElement contentContainer => m_ListContentContainer ?? base.contentContainer;
        #endregion

        protected VisualElement m_ListContentContainer;
        protected SerializedObject m_SerializedObject;

        protected readonly ListElementsFactory m_ListElementsFactory;

        private string m_ListPropertyBindingPath;

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

                field.m_ListElementsFactory.Init();
            }
        }

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
                                    });
                            }

                            if (!DisablePropertyLabel && DisableLabelContextMenu)
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