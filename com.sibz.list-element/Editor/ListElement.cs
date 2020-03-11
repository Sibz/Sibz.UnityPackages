using System;
using System.Collections.Generic;
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
            public const string DefaultTemplateName = "Sibz.ListElement.Template";
            public const string DefaultItemTemplateName = "Sibz.ListElement.ItemTemplate";
            public const string DefaultStyleSheetName = "Sibz.ListElement.Template";
            public const string HeaderSectionClassName = "sibz-list-header";
            public const string DeleteConfirmSectionClassName = "sibz-list-delete-all-confirm";
            public const string ItemSectionClassName = "sibz-list-items-section";
            public const string HeaderLabelClassName = "sibz-list-header-label";
            public const string DeleteAllButtonClassName = "sibz-list-delete-all-button";
            public const string AddButtonClassName = "sibz-list-add-button";
            public const string DeleteConfirmButtonClassName = "sibz-list-delete-confirm-yes";
            public const string DeleteCancelButtonClassName = "sibz-list-delete-confirm-no";
            public const string DeleteItemButtonClassName = "sibz-list-delete-item-button";
            public const string MoveUpButtonClassName = "sibz-list-move-up-button";
            public const string MoveDownButtonClassName = "sibz-list-move-down-button";


            public string TemplateName { get; set; } = DefaultTemplateName;
            public string ItemTemplateName { get; set; } = DefaultItemTemplateName;
            public string StyleSheetName { get; set; } = DefaultStyleSheetName;
            public string Label { get; set; }
        }

        private VisualTreeAsset itemTemplate;
        private StyleSheet styleSheet;
        private VisualTreeAsset template;
        private SerializedProperty serializedProperty;

        private List<ButtonBinder> outsideButtonBinders;

        public class AddNewEvent : EventBase<AddNewEvent>
        {
        }

        public class DeleteAllEvent : EventBase<DeleteAllEvent>
        {
        }

        public class DeleteAllConfirmEvent : EventBase<DeleteAllConfirmEvent>
        {
        }

        public class DeleteAllCancelEvent : EventBase<DeleteAllCancelEvent>
        {
        }

        public string Label { get; set; }
        public string TemplateName { get; set; }
        public string ItemTemplateName { get; set; }
        public string StyleSheetName { get; set; }
        public bool IsInitialised { get; private set; }
        public Type ListItemType { get; private set; }
        public event Action OnReset;

        public ListElement() : this(null, new Config())
        {
        }

        public ListElement(SerializedProperty property) : this(property, new Config())
        {
        }

        public ListElement(SerializedProperty property, string label) : this(property, new Config() {Label = label})
        {
        }

        public ListElement(SerializedProperty property, Config conf)
        {
            CreateButtonBinders();

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

        private void CreateButtonBinders()
        {
            void RaiseEventForButton<T>() where T : EventBase, new()
            {
                SendEvent(new T() {target = this});
            }

            outsideButtonBinders = new List<ButtonBinder>()
            {
                new ButtonBinder(Config.AddButtonClassName, RaiseEventForButton<AddNewEvent>),
                new ButtonBinder(Config.DeleteAllButtonClassName, RaiseEventForButton<DeleteAllEvent>),
                new ButtonBinder(Config.DeleteConfirmButtonClassName, RaiseEventForButton<DeleteAllConfirmEvent>),
                new ButtonBinder(Config.DeleteCancelButtonClassName, RaiseEventForButton<DeleteAllCancelEvent>),
            };
        }

        private void BindInsideButtons(int index, VisualElement itemSection)
        {
            void RaiseEventForButton<T>() where T : ItemButtonEventBase, new()
            {
                SendEvent(new T() {target = this, index = index });
            }

            new ButtonBinder(Config.DeleteItemButtonClassName, RaiseEventForButton<ItemDeleteEvent>).BindToFunction(itemSection);
            new ButtonBinder(Config.MoveUpButtonClassName, RaiseEventForButton<ItemMoveUpEvent>).BindToFunction(itemSection);
            new ButtonBinder(Config.MoveDownButtonClassName, RaiseEventForButton<ItemMoveDownEvent>).BindToFunction(itemSection);
            
        }

        public class ItemButtonEventBase : EventBase<ItemButtonEventBase>
        {
            public int index;
        }

        public class ItemMoveUpEvent : ItemButtonEventBase
        {
        }
        public class  ItemMoveDownEvent : ItemButtonEventBase
        {            
        }
        public class ItemDeleteEvent : ItemButtonEventBase
        {
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

            LoadTemplate();

            BindOutsideButtonsAndRegisterCallbacks();

            SetLabelText();
            
            if (!(serializedProperty is null))
            {
                InitialiseWithSerializedProperty();
            }
        }

        private void InitialiseWithSerializedProperty()
        {
            if (TryGetItemType(serializedProperty, out Type listItemType))
            {
                ListItemType = listItemType;
            }
            
            AddArraySizeField();

            LoadItemTemplate();
        }

        private void BindOutsideButtonsAndRegisterCallbacks()
        {
            outsideButtonBinders.BindButtons(this);
            RegisterCallback<AddNewEvent>(AddNewItem);
            RegisterCallback<DeleteAllEvent>(DeleteAllClicked);
            RegisterCallback<DeleteAllConfirmEvent>(DeleteAllConfirmed);
            RegisterCallback<DeleteAllCancelEvent>(DeleteAllCancelled);
        }

        private void PopulateList()
        {
            if (!serializedProperty.isArray)
            {
                return;
            }

            int length = serializedProperty.arraySize;
            VisualElement listContainer = this.Q<VisualElement>(null, Config.ItemSectionClassName);
            listContainer.Clear();
            for (int i = 0; i < length; i++)
            {
                VisualElement listItemElement = new VisualElement();
                itemTemplate.CloneTree(listItemElement);
                listItemElement.Q<PropertyField>().BindProperty(serializedProperty.GetArrayElementAtIndex(i));
                listContainer.Add(listItemElement);
                BindInsideButtons(i, listItemElement);
            }
        }

        private void SetLabelText()
        {
            Label label = this.Query<Label>(null, Config.HeaderLabelClassName);
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

        private void LoadTemplate()
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

        private static bool TryGetItemType(SerializedProperty property, out Type type)
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

            type = baseObject.GetType().IsGenericType
                ? baseObject.GetType().GetGenericArguments()[0]
                : baseObject.GetType();

            return true;
        }

        protected override void ExecuteDefaultAction(EventBase evt)
        {
            base.ExecuteDefaultAction(evt);
            Type type = evt.GetType();
            if (type.Name == "SerializedPropertyBindEvent"
                &&
                type.GetProperty("bindProperty")?.GetValue(evt) is SerializedProperty property)
            {
                serializedProperty = property;
            }
        }

        public void DeleteItem(int index)
        {
            if (index < 0 || index >= serializedProperty.arraySize)
            {
                throw new IndexOutOfRangeException("Unable to delete item");
            }
            serializedProperty.DeleteArrayElementAtIndex(index);
            serializedProperty.serializedObject.ApplyModifiedProperties();
            Reset();
        }

        public void MoveItemUp(int index)
        {
            if (index == 0)
            {
                return;
            }
            if (index < 0  || index >= serializedProperty.arraySize)
            {
                throw new IndexOutOfRangeException("Unable to move item");
            }

            serializedProperty.MoveArrayElement(index, index - 1);
            serializedProperty.serializedObject.ApplyModifiedProperties();
            Reset();
        }
        
        public void MoveItemDown(int index)
        {
            if (index == serializedProperty.arraySize -1)
            {
                return;
            }
            if (index < 0  || index >= serializedProperty.arraySize)
            {
                throw new IndexOutOfRangeException("Unable to move item");
            }

            serializedProperty.MoveArrayElement(index, index + 1);
            serializedProperty.serializedObject.ApplyModifiedProperties();
            Reset();
        }

        private void AddNewItem(AddNewEvent evt)
        {
            AddNewItemToList();
        }

        public void AddNewItemToList()
        {
            serializedProperty.InsertArrayElementAtIndex(serializedProperty.arraySize);
            serializedProperty.serializedObject.ApplyModifiedProperties();
            Reset();
        }

        private void DeleteAllClicked(DeleteAllEvent evt)
        {
            ToggleDeleteAll(true);
        }

        private void DeleteAllConfirmed(DeleteAllConfirmEvent evt)
        {
            ToggleDeleteAll();
            ClearListItems();
        }

        private void DeleteAllCancelled(DeleteAllCancelEvent evt)
        {
            ToggleDeleteAll();
        }

        private void ToggleDeleteAll(bool show = false)
        {
            this.Q(null, Config.DeleteConfirmSectionClassName).style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
            this.Q(null, Config.DeleteAllButtonClassName).style.display = show ? DisplayStyle.None : DisplayStyle.Flex;
            this.Q(null, Config.AddButtonClassName).style.display =show ? DisplayStyle.None : DisplayStyle.Flex;
        }

        public void ClearListItems()
        {
            serializedProperty.ClearArray();
            serializedProperty.serializedObject.ApplyModifiedProperties();
            Reset();
        }

        public new class UxmlFactory : UxmlFactory<ListElement, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription label;
            private readonly UxmlStringAttributeDescription itemTemplateName;
            private readonly UxmlStringAttributeDescription styleSheetName;
            private readonly UxmlStringAttributeDescription templateName;

            public UxmlTraits()
            {
                label = new UxmlStringAttributeDescription {name = "label"};
                itemTemplateName = new UxmlStringAttributeDescription {name = "item-template-name"};
                styleSheetName = new UxmlStringAttributeDescription {name = "stylesheet-name"};
                templateName = new UxmlStringAttributeDescription {name = "template-name"};
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