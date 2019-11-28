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

        public const string ADD_BUTTON_TEXT = "+";
        public const string DELETE_ALL_BUTTON_TEXT = "Clear List";
        public const string DELETE_ALL_CONFIRM_LABEL_TEXT = "Are you sure?";
        public const string DELETE_ALL_YES_BUTTON_TEXT = "Yes";
        public const string DELETE_ALL_NO_BUTTON_TEXT = "No";
        public const string DELETE_ITEM_BUTTON_TEXT = "-";
        public const string MOVE_UP_BUTTON_TEXT = "↑";
        public const string MOVE_DOWN_BUTTON_TEXT = "↓";

        #region Public Properties
        public string Label { get; set; }
        public bool ShowSize { get; set; }
        public bool DisableLabelContextMenu { get; set; }
        public bool DisablePropertyLabel { get; set; }

        public bool HideAddButton { get; set; }
        public bool HideDeleteAllButton { get; set; }
        public bool HideDeleteItemButton { get; set; }
        public bool HideReorderItemButtons { get; set; }

        public string AddButtonText { get; set; } = ADD_BUTTON_TEXT;
        public string DeleteAllButtonText { get; set; } = DELETE_ALL_YES_BUTTON_TEXT;
        public string DeleteAllConfirmLabelText { get; set; } = DELETE_ALL_CONFIRM_LABEL_TEXT;
        public string DeleteAllYesButtonText { get; set; } = DELETE_ALL_YES_BUTTON_TEXT;
        public string DeleteAllNoButtonText { get; set; } = DELETE_ALL_NO_BUTTON_TEXT;


        public string DeleteItemButtonText { get; set; } = DELETE_ITEM_BUTTON_TEXT;
        public string ReorderItemUpButtonText { get; set; } = MOVE_UP_BUTTON_TEXT;
        public string ReorderItemDownButtonText { get; set; } = MOVE_DOWN_BUTTON_TEXT;


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
            //UxmlBoolAttributeDescription m_ShowSize;
            UxmlBoolAttributeDescription m_DisableLabelContextMenu;
            UxmlBoolAttributeDescription m_DisablePropertyLabel;

            UxmlBoolAttributeDescription m_HideAddButton;
            UxmlBoolAttributeDescription m_HideDeleteAllButton;
            UxmlBoolAttributeDescription m_HideDeleteItemButton;
            UxmlBoolAttributeDescription m_HideReorderItemButtons;

            UxmlStringAttributeDescription m_AddButtonText;
            UxmlStringAttributeDescription m_DeleteAllButtonText;
            UxmlStringAttributeDescription m_DeleteAllConfirmLabelText;
            UxmlStringAttributeDescription m_DeleteAllYesButtonText;
            UxmlStringAttributeDescription m_DeleteAllNoButtonText;
            UxmlStringAttributeDescription m_DeleteItemButtonText;
            UxmlStringAttributeDescription m_ReorderItemUpButtonText;
            UxmlStringAttributeDescription m_ReorderItemDownButtonText;



            public UxmlTraits()
            {
                m_PropertyPath = new UxmlStringAttributeDescription { name = "binding-path" };
                m_Label = new UxmlStringAttributeDescription { name = "label" };
                //m_ShowSize = new UxmlBoolAttributeDescription { name = "show-size" };
                m_DisableLabelContextMenu = new UxmlBoolAttributeDescription { name = "disable-label-context-menu" };
                m_DisablePropertyLabel = new UxmlBoolAttributeDescription { name = "disable-property-label" };

                m_HideAddButton = new UxmlBoolAttributeDescription { name = "hide-add-button"};
                m_HideDeleteAllButton = new UxmlBoolAttributeDescription { name = "hide-delete-all-button"};
                m_HideDeleteItemButton = new UxmlBoolAttributeDescription { name = "hide-delete-item-button" };
                m_HideReorderItemButtons = new UxmlBoolAttributeDescription { name = "hide-reorder-item-buttons" };

                m_AddButtonText = new UxmlStringAttributeDescription { name = "add-button-text", defaultValue = ADD_BUTTON_TEXT };
                m_DeleteAllButtonText = new UxmlStringAttributeDescription { name = "delete-all-button-text", defaultValue = DELETE_ALL_BUTTON_TEXT };
                m_DeleteAllConfirmLabelText = new UxmlStringAttributeDescription { name = "delete-all-confirm-label-text", defaultValue = DELETE_ALL_CONFIRM_LABEL_TEXT };
                m_DeleteAllYesButtonText = new UxmlStringAttributeDescription { name = "delete-all-yes-button-text", defaultValue = DELETE_ALL_YES_BUTTON_TEXT };
                m_DeleteAllNoButtonText = new UxmlStringAttributeDescription { name = "delete-all-no-button-text", defaultValue = DELETE_ALL_NO_BUTTON_TEXT };
                m_DeleteItemButtonText = new UxmlStringAttributeDescription { name = "delete-item-button-text", defaultValue = DELETE_ITEM_BUTTON_TEXT };
                m_ReorderItemUpButtonText = new UxmlStringAttributeDescription { name = "reorder-item-up-button-text", defaultValue = MOVE_UP_BUTTON_TEXT };
                m_ReorderItemDownButtonText = new UxmlStringAttributeDescription { name = "reorder-item-down-button-text", defaultValue = MOVE_DOWN_BUTTON_TEXT };

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

                field.Label = m_Label.GetValueFromBag(bag, cc);
                field.DisableLabelContextMenu = m_DisableLabelContextMenu.GetValueFromBag(bag, cc);
                field.DisablePropertyLabel = m_DisablePropertyLabel.GetValueFromBag(bag, cc);

                field.HideAddButton = m_HideAddButton.GetValueFromBag(bag, cc);
                field.HideDeleteAllButton = m_HideDeleteAllButton.GetValueFromBag(bag, cc);
                field.HideDeleteItemButton = m_HideDeleteItemButton.GetValueFromBag(bag, cc);
                field.HideReorderItemButtons = m_HideReorderItemButtons.GetValueFromBag(bag, cc);

                field.AddButtonText = m_AddButtonText.GetValueFromBag(bag, cc);
                field.DeleteAllButtonText = m_DeleteAllButtonText.GetValueFromBag(bag, cc);
                field.DeleteAllConfirmLabelText = m_DeleteAllConfirmLabelText.GetValueFromBag(bag, cc);
                field.DeleteAllYesButtonText = m_DeleteAllYesButtonText.GetValueFromBag(bag, cc);
                field.DeleteAllNoButtonText = m_DeleteAllNoButtonText.GetValueFromBag(bag, cc);
                field.DeleteItemButtonText = m_DeleteItemButtonText.GetValueFromBag(bag, cc);
                field.ReorderItemUpButtonText = m_ReorderItemUpButtonText.GetValueFromBag(bag, cc);
                field.ReorderItemDownButtonText = m_ReorderItemDownButtonText.GetValueFromBag(bag, cc);

                field.m_ListElementsFactory.Init();
            }
        }

        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultActionAtTarget(evt);
            //m_ListElementsFactory.Reset();
            //evt.StopPropagation();
            System.Type type = evt.GetType();
            if (type.Name == "SerializedPropertyBindEvent"
                &&
                type.GetProperty("bindProperty").GetValue(evt) is SerializedProperty property)
            {


                m_SerializedObject = property.serializedObject;
                m_ListPropertyBindingPath = ((ListVisualElement)evt.target).bindingPath;

                m_ListElementsFactory.Reset();
                // Don't allow the binding of `this` to continue because `this` is not
                // the actually bound field, it is just a container.
                evt.StopPropagation();
            }
        }

        //private void Reset(SerializedProperty prop)
        //{

        //    m_ListContentContainer.Clear();
        //    if (prop.isArray)
        //    {
        //        m_ListElementsFactory.Controls.DeleteAllButton.SetEnabled(prop.arraySize > 0);
        //        var endProperty = prop.GetEndProperty();

        //        prop.NextVisible(true);
        //        do
        //        {

        //            if (SerializedProperty.EqualContents(prop, endProperty))
        //            {
        //                break;
        //            }

        //            switch (prop.propertyType)
        //            {
        //                case SerializedPropertyType.ArraySize:
        //                    var field = new IntegerField { bindingPath = prop.propertyPath };
        //                    field.SetValueWithoutNotify(prop.intValue); // This avoids the OnValueChanged/Rebind feedback loop.
        //                    field.style.display = ShowSize ? DisplayStyle.Flex : DisplayStyle.None;
        //                    field.RegisterValueChangedCallback(UpdateList);
        //                    field.label = "Size";
        //                    m_ListContentContainer.Add(field);
        //                    break;

        //                default:
        //                    var f = new PropertyField(prop);
        //                    m_ListContentContainer.Add(f);
        //                    if (DisablePropertyLabel)
        //                    {
        //                        f.RegisterCallback<AttachToPanelEvent>((e) =>
        //                            {

        //                                if (f.Q<Label>() is Label)
        //                                {
        //                                    f.Q<Label>().style.display = DisplayStyle.None;
        //                                }
        //                            });
        //                    }

        //                    if (!DisablePropertyLabel && DisableLabelContextMenu)
        //                    {
        //                        f.RegisterCallback<MouseUpEvent>((e) =>
        //                            {
        //                                if (e.target is Label && ((Label)e.target).parent?.parent == f)
        //                                {
        //                                    e.StopPropagation();
        //                                }
        //                            }, TrickleDown.TrickleDown);
        //                    }

        //                    break;
        //            }

        //        } while (prop.NextVisible(false));

        //        prop.Reset();

        //    }
        //    else
        //    {
        //        m_ListContentContainer.Add(new Label("Error, Bound item is not a list or array"));
        //    }
        //}

        //private void UpdateList(ChangeEvent<int> changeEvent)
        //{
        //    this.Unbind();
        //    m_SerializedObject.UpdateIfRequiredOrScript();
        //    m_SerializedObject.ApplyModifiedProperties();
        //    this.Bind(m_SerializedObject);

        //    // Enable/Disable Delete All Button
        //    m_ListElementsFactory.Controls.DeleteAllButton.SetEnabled(ListProperty.arraySize > 0);
        //    changeEvent.StopImmediatePropagation();
        //}
    }
}