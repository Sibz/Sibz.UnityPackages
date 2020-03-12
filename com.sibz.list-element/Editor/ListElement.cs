using System;
using Sibz.ListElement.Events;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

namespace Sibz.ListElement
{
    public class ListElement : BindableElement
    {
        private VisualTreeAsset template;
        private StyleSheet styleSheet;
        private VisualTreeAsset itemTemplate;
        private SerializedProperty serializedProperty;
        private PropertyModificationHandler modHandler;
        private readonly ListElementEventHandler eventHandler;

        #region Attribute Properties

        public string Label { get; private set; }
        public string TemplateName { get; private set; }
        public string ItemTemplateName { get; private set; }
        public string StyleSheetName { get; private set; }
        private bool HidePropertyLabel { get; set; }

        #endregion

        public bool IsInitialised { get; private set; }
        public Type ListItemType { get; private set; }
        public event Action OnReset;

        #region Construction

        public ListElement() : this(null, new ListElementOptions())
        {
        }

        public ListElement(SerializedProperty property) : this(property, new ListElementOptions())
        {
        }

        public ListElement(SerializedProperty property, string label) : this(property,
            new ListElementOptions() {Label = label})
        {
        }

        public ListElement(SerializedProperty property, ListElementOptions options)
        {
            serializedProperty = property;

            eventHandler = new ListElementEventHandler(this, modHandler);

            ImportOptions(options);

            InitialiseAndBindIfPropertyIsDefined();
        }

        private void ImportOptions(ListElementOptions options)
        {
            Label = options.Label;
            TemplateName = options.TemplateName;
            ItemTemplateName = options.ItemTemplateName;
            StyleSheetName = options.StyleSheetName;
            HidePropertyLabel = options.HidePropertyLabel;
        }

        private void InitialiseAndBindIfPropertyIsDefined()
        {
            if (serializedProperty is null)
            {
                return;
            }

            Initialise();

            this.BindProperty(serializedProperty);
        }

        #endregion

        private void Initialise()
        {
            Clear();

            LoadAndCloneTemplate();

            ImportStyleSheetIfCustom();

            eventHandler.BindOuterButtons();

            SetLabelText();

            AddHidePropertyStyleSheetIfRequired();

            InitialiseWithSerializedProperty();
        }

        private void LoadAndCloneTemplate()
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

        private void ImportStyleSheetIfCustom()
        {
            if (StyleSheetName != TemplateName)
            {
                styleSheets.Add(SingleAssetLoader.SingleAssetLoader.Load<StyleSheet>(StyleSheetName));
            }
        }

        private void SetLabelText()
        {
            Label label = this.Query<Label>(null, Constants.HeaderLabelClassName);
            if (string.IsNullOrEmpty(Label) && !(serializedProperty is null))
            {
                label.text = ObjectNames.NicifyVariableName(serializedProperty.name);
            }
            else if (string.IsNullOrEmpty(Label) && serializedProperty is null)
            {
                label.text = "<List Name>";
            }
            else
            {
                label.text = Label;
            }
        }

        private void AddHidePropertyStyleSheetIfRequired()
        {
            if (HidePropertyLabel)
            {
                styleSheets.Add(
                    SingleAssetLoader.SingleAssetLoader.Load<StyleSheet>(Constants.HidePropertyLabelStyleSheetName));
            }
        }

        private void InitialiseWithSerializedProperty()
        {
            if (serializedProperty is null)
            {
                return;
            }

            ListItemType = GetItemType(serializedProperty);

            modHandler = new PropertyModificationHandler(serializedProperty, Reset);

            AddArraySizeField();

            LoadItemTemplate();

            IsInitialised = true;
        }

        private static Type GetItemType(SerializedProperty property)
        {
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
                return null;
            }

            return baseObject.GetType().IsGenericType
                ? baseObject.GetType().GetGenericArguments()[0]
                : baseObject.GetType();
        }

        private void AddArraySizeField()
        {
            IntegerField integerField = new IntegerField
            {
                bindingPath = serializedProperty.FindPropertyRelative("Array.size").propertyPath
            };

            integerField.style.display = DisplayStyle.None;

            void DoReset(ChangeEvent<int> evt)
            {
                Reset();
            }

            integerField.RegisterCallback<ChangeEvent<int>>(DoReset);

            Add(integerField);
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

            if (type.Name != "SerializedPropertyBindEvent" ||
                !(type.GetProperty("bindProperty")?.GetValue(evt) is SerializedProperty property))
            {
                return;
            }

            serializedProperty = property;
            if (!IsInitialised)
            {
                Initialise();
            }

            Reset();
        }

        private void Reset()
        {
            PopulateList();

            OnReset?.Invoke();
        }

        private void PopulateList()
        {
            if (!serializedProperty.isArray)
            {
                Debug.LogWarning("Bound property is not an array type");
                return;
            }

            VisualElement listContainer = this.Q<VisualElement>(null, Constants.ItemSectionClassName);
            listContainer.Clear();

            for (int i = 0; i < serializedProperty.arraySize; i++)
            {
                VisualElement listItemElement = new VisualElement();
                itemTemplate.CloneTree(listItemElement);
                listItemElement.Q<PropertyField>().BindProperty(serializedProperty.GetArrayElementAtIndex(i));
                listContainer.Add(listItemElement);
                eventHandler.BindItemButtons(i, listItemElement);
            }
        }

        public void RemoveItem(int index)
        {
            modHandler.Remove(index);
        }

        public void MoveItemUp(int index)
        {
            modHandler.MoveUp(index);
        }

        public void MoveItemDown(int index)
        {
            modHandler.MoveDown(index);
        }

        public void AddNewItemToList()
        {
            modHandler.Add();
        }

        public void ClearListItems()
        {
            modHandler.Clear();
        }

        // ReSharper disable once UnusedMember.Global
        public new class UxmlFactory : UxmlFactory<ListElement, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription label;
            private readonly UxmlStringAttributeDescription itemTemplateName;
            private readonly UxmlStringAttributeDescription styleSheetName;
            private readonly UxmlStringAttributeDescription templateName;
            private readonly UxmlBoolAttributeDescription hidePropertyLabel;

            public UxmlTraits()
            {
                label = new UxmlStringAttributeDescription {name = "label"};
                itemTemplateName = new UxmlStringAttributeDescription {name = "item-template-name"};
                styleSheetName = new UxmlStringAttributeDescription {name = "stylesheet-name"};
                templateName = new UxmlStringAttributeDescription {name = "template-name"};
                hidePropertyLabel = new UxmlBoolAttributeDescription() {name = "hide-property-label"};
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                if (!(ve is ListElement le))
                {
                    return;
                }

                le.Label = label.GetValueFromBag(bag, cc);
                le.HidePropertyLabel = hidePropertyLabel.GetValueFromBag(bag, cc);

                string itn = itemTemplateName.GetValueFromBag(bag, cc);
                string ssn = styleSheetName.GetValueFromBag(bag, cc);
                string tn = templateName.GetValueFromBag(bag, cc);

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