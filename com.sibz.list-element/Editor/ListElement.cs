﻿using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sibz.ListElement
{
    /// <summary>
    /// TODO
    /// Must initialise when constructed with SP
    /// Must initialise after values loaded from Uxml
    /// Must reset the list when SP changes
    /// On Initialise must load Templates and style
    /// On Initialise must insert main template and apply style
    /// On Initialise must bind events to main buttons (Add/Clear List/Yes/Cancel)
    /// On Reset, insert SP bound row, with bound buttons (Delete/MoveUp/MoveDown)
    ///  Provide events for:
    ///   Reset
    ///   Initialise
    ///   Row Created
    /// </summary>
    public class ListElement : BindableElement
    {
        private const string DefaultTemplateName = "Sibz.ListElement.Template";
        private const string DefaultItemTemplateName = "Sibz.ListElement.ItemTemplate";
        private const string DefaultStyleSheetName = "Sibz.ListElement.Template";
        private VisualTreeAsset itemTemplate;
        private StyleSheet styleSheet;
        private VisualTreeAsset template;

        public bool HideLabel { get; set; }
        public string Label { get; set; } = "";
        public string TemplateName { get; set; } = DefaultTemplateName;
        public string ItemTemplateName { get; set; } = DefaultItemTemplateName;
        public string StyleSheetName { get; set; } = DefaultStyleSheetName;

        public bool IsInitialised { get; private set; }
        public event Action OnReset;

        public ListElement() : this(null, null)
        {
        }

        public ListElement(SerializedProperty property) : this(property, string.Empty)
        {
        }

        public ListElement(SerializedProperty property, string label)
        {
            if (!string.IsNullOrEmpty(label))
            {
                Label = label;
            }

            if (property is null)
            {
                return;
            }

            Initialise();

            this.BindProperty(property);
        }

        private void AddArraySizeField(SerializedProperty property)
        {
            IntegerField integerField = new IntegerField
            {
                bindingPath = property.FindPropertyRelative("Array.size").propertyPath
            };
            integerField.style.display = DisplayStyle.None;
            integerField.RegisterCallback<ChangeEvent<int>>(x => Reset(property));
            Add(integerField);
        }

        private void Initialise()
        {
            IsInitialised = true;
        }

        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultActionAtTarget(evt);
            Type type = evt.GetType();
            if (type.Name == "SerializedPropertyBindEvent"
                &&
                type.GetProperty("bindProperty")?.GetValue(evt) is SerializedProperty property)
            {
                Reset(property);
            }
        }

        private void Reset(SerializedProperty property)
        {
            Clear();
            AddArraySizeField(property);
            OnReset?.Invoke();
        }

        public new class UxmlFactory : UxmlFactory<ListElement, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableElement.UxmlTraits
        {
/*            private UxmlBoolAttributeDescription hideLabel;
            private UxmlStringAttributeDescription itemTemplateName;
            private UxmlStringAttributeDescription label;
            private UxmlStringAttributeDescription styleSheetName;
            private UxmlStringAttributeDescription templateName;*/

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is ListElement le)
                {
                    le.Initialise();
                }
            }
        }
    }
}