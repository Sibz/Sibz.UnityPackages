using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

namespace Sibz.ListElement
{
    public class ListElement : BindableElement
    {
        public class Config
        {
            public string TemplateName { get; set; } = "Sibz.ListElement.Template";
            public string ItemTemplateName { get; set; }= "Sibz.ListElement.ItemTemplate";
            public string StyleSheetName { get; set; }= "Sibz.ListElement.Template";
            public string Label { get; set; }
        }
        
        private VisualTreeAsset itemTemplate;
        private StyleSheet styleSheet;
        private VisualTreeAsset template;
        private SerializedProperty serializedProperty;

        public string Label { get; set; }
        public string TemplateName { get; set; }
        public string ItemTemplateName { get; set; }
        public string StyleSheetName { get; set; }
        public bool IsInitialised { get; private set; }
        public Type ListItemType { get; private set; }
        public event Action OnReset;

        public ListElement() : this(null, new Config()){}
        public ListElement(SerializedProperty property) : this(property, new Config()){}
        public ListElement(SerializedProperty property, string label) : this(property, new Config() { Label =  label }){}
        public ListElement(SerializedProperty property, Config conf)
        {
            Label = conf.Label;
            TemplateName = conf.TemplateName;
            ItemTemplateName = conf.ItemTemplateName;
            StyleSheetName = conf.StyleSheetName;
            if (StyleSheetName != TemplateName)
            {
                styleSheets.Add(SingleAssetLoader.SingleAssetLoader.Load<StyleSheet>(StyleSheetName));
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

            SetLabelText();
            
            if (serializedProperty is null)
            {
                return;
            }

            if (TryGetItemType(serializedProperty, out Type listItemType))
            {
                ListItemType = listItemType;
            }

            AddArraySizeField();
            
            LoadItemTemplate();
            
            PopulateList();
        }

        private void PopulateList()
        {
            if (!serializedProperty.isArray || serializedProperty.arraySize == 0)
            {
                return;
            }

            int length = serializedProperty.arraySize;
            VisualElement listContainer = this.Q<VisualElement>(null, "sibz-list-items-section");
            for (int i = 0; i < length; i++)
            {
                VisualElement listItemElement = new VisualElement();
                itemTemplate.CloneTree(listItemElement);
                listItemElement.Q<PropertyField>().BindProperty(serializedProperty.GetArrayElementAtIndex(i));
                listContainer.Add(listItemElement);
            }
        }

        private void SetLabelText()
        {
            Label label = this.Query<Label>(null, "sibz-list-header-label");
            if (string.IsNullOrEmpty(Label) && !(serializedProperty is null))
            {
                label.text = ObjectNames.NicifyVariableName(serializedProperty.name);
            }
            else if (string.IsNullOrEmpty(Label) && (serializedProperty is null))
            {
                label.text = "<List Name>";
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

        private void LoadItemTemplate()
        {
            try
            {
                itemTemplate = SingleAssetLoader.SingleAssetLoader.Load<VisualTreeAsset>(ItemTemplateName);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat(
                    "Unable to load item template ('{0}'): {1}", 
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

        private bool TryGetItemType(SerializedProperty property, out Type type)
        {
            
            type = null;
            
            var propertyPath = property.propertyPath.Split('.');
            object baseObject = property.serializedObject.targetObject;
            foreach (string fieldName in propertyPath)
            {
                baseObject = baseObject.GetType().GetField(fieldName)?.GetValue(baseObject);
                if (baseObject != null)
                {
                    continue;
                }

                Debug.LogWarning($"Unable to get item type. Field {fieldName} does not exist on object");
                return false;
            }

            type = baseObject.GetType().IsGenericType ? baseObject.GetType().GetGenericArguments()[0] : baseObject.GetType();

            return true;
        }
        
        protected override void ExecuteDefaultAction(EventBase evt)
        {
            base.ExecuteDefaultAction(evt);
            System.Type type = evt.GetType();
            if (type.Name == "SerializedPropertyBindEvent"
                &&
                type.GetProperty("bindProperty")?.GetValue(evt) is SerializedProperty property)
            {
                serializedProperty = property;
            }
        }

        public new class UxmlFactory : UxmlFactory<ListElement, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            
            private UxmlStringAttributeDescription label;
            private UxmlStringAttributeDescription itemTemplateName;
            private UxmlStringAttributeDescription styleSheetName;
            private UxmlStringAttributeDescription templateName;

            public UxmlTraits()
            {
                label = new UxmlStringAttributeDescription { name = "label" };
                itemTemplateName = new UxmlStringAttributeDescription { name = "item-template-name" };
                styleSheetName = new UxmlStringAttributeDescription { name = "stylesheet-name" };
                templateName = new UxmlStringAttributeDescription { name = "template-name" };
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                if (!(ve is ListElement le))
                {
                    return;
                }

                string lbl = label.GetValueFromBag(bag, cc);
                string itn = itemTemplateName.GetValueFromBag(bag, cc);
                string ssn = styleSheetName.GetValueFromBag(bag, cc);
                string tn = templateName.GetValueFromBag(bag, cc);
                
                le.Label = lbl;
 
                if (!string.IsNullOrEmpty(itn))
                {
                    le.ItemTemplateName = itn;
                }
                
                if (!string.IsNullOrEmpty(ssn))
                {
                    le.StyleSheetName = ssn;
                }
                
                if (!string.IsNullOrEmpty(tn))
                {
                    le.TemplateName = tn;
                }
                
                le.Initialise();
            }
        }
    }
}