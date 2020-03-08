using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Sibz.SingleAssetLoader;
using UnityEngine;

namespace Sibz.ListElement
{
    public class ListElement : BindableElement
    {
        private const string DefaultTemplateName = "Sibz.ListElement.Template";
        private const string DefaultItemTemplateName = "Sibz.ListElement.ItemTemplate";
        private const string DefaultStyleSheetName = "Sibz.ListElement.Template";
        private VisualTreeAsset itemTemplate;
        private StyleSheet styleSheet;
        private VisualTreeAsset template;

        public string Label { get; set; } = "";
        public string TemplateName { get; set; } = DefaultTemplateName;
        public string ItemTemplateName { get; set; } = DefaultItemTemplateName;
        public string StyleSheetName { get; set; } = DefaultStyleSheetName;

        private SerializedProperty serializedProperty;
        
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

            serializedProperty = property;
            
            Initialise();

            this.BindProperty(property);
        }

        private void AddArraySizeField()
        {
            IntegerField integerField = new IntegerField
            {
                bindingPath = serializedProperty.FindPropertyRelative("Array.size").propertyPath
            };
            
            integerField.style.display = DisplayStyle.None;
            
            integerField.RegisterCallback<ChangeEvent<int>>(x => Reset());
            
            Add(integerField);
        }

        private void Initialise()
        {
            IsInitialised = true;
            
            Clear();

            CloneTemplate();

            if (serializedProperty is null)
            {
                return;
            }

            SetLabelText();
            
            AddArraySizeField();
        }

        private void SetLabelText()
        {
            Label label = this.Query<Label>(null, "sibz-list-header-label");
            if (string.IsNullOrEmpty(Label))
            {
                label.text = ObjectNames.NicifyVariableName(serializedProperty.name);
            }
            else
            {
                label.text = Label;
            }
        }

      

        private void CloneTemplate()
        {
            try
            {
                SingleAssetLoader.SingleAssetLoader.Load<VisualTreeAsset>(TemplateName).CloneTree(this);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat(
                    "Unable to load template ('{0}') to clone into ListElement: {1}", 
                    TemplateName,
                    e.Message);
            }
            
            
            
        }
        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultActionAtTarget(evt);
            Type type = evt.GetType();
            if (type.Name == "SerializedPropertyBindEvent"
                &&
                type.GetProperty("bindProperty")?.GetValue(evt) is SerializedProperty property)
            {
                serializedProperty = property;
                Reset();
            }
        }

        private void Reset()
        {
            
          Initialise();
          
            OnReset?.Invoke();
        }

        public new class UxmlFactory : UxmlFactory<ListElement, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableElement.UxmlTraits
        {
/*           
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