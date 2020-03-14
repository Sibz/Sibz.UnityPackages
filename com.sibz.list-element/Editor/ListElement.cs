using System;
using System.Diagnostics.CodeAnalysis;
using Sibz.ListElement.Events;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Sibz.ListElement
{
    public class ListElement : BindableElement
    {
        private readonly IListElementEventHandler eventHandler;
        private VisualTreeAsset itemTemplate;
        private SerializedProperty serializedProperty;
        private StyleSheet styleSheet;
        private VisualTreeAsset template;

        public bool IsInitialised { get; private set; }
        public Type ListItemType { get; private set; }
        public event Action OnReset;

        #region Attribute Properties

        public string Label { get; private set; }
        public string TemplateName { get; private set; }
        public string ItemTemplateName { get; private set; }
        public string StyleSheetName { get; private set; }
        private bool HidePropertyLabel { get; set; }
        private bool DoNotUseObjectField { get; set; }

        #endregion

        #region Construction

        public ListElement() : this(null)
        {
        }

        public ListElement(IListElementEventHandler evtHandler) : this(null, new ListElementOptions(), evtHandler)
        {
        }

        public ListElement(SerializedProperty property, IListElementEventHandler evtHandler = null) : this(property,
            new ListElementOptions(), evtHandler)
        {
        }

        public ListElement(SerializedProperty property, string label, IListElementEventHandler evtHandler = null) :
            this(property,
                new ListElementOptions {Label = label}, evtHandler)
        {
        }

        public ListElement(SerializedProperty property, ListElementOptions options,
            IListElementEventHandler evtHandler = null)
        {
            serializedProperty = property;

            eventHandler = evtHandler ?? new ListElementEventHandler(this);

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
            DoNotUseObjectField = options.DoNotUseObjectField;
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

        #region Initialisation

        private void Initialise()
        {
            Clear();

            LoadAndCloneTemplate();

            ImportStyleSheetIfCustom();

            SetLabelText();

            SetObjectFieldLabelText();

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
            Label label = this.Q<Label>(null, Constants.HeaderLabelClassName);
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

        private void SetObjectFieldLabelText()
        {
            if (!(GetLabelInHierarchy() is Label label))
            {
                return;
            }

            label.parent.style.justifyContent = Justify.Center;
            label.style.display = DisplayStyle.None;
            label.parent.Add(new Label("Drop here to add new item") {pickingMode = PickingMode.Ignore});
        }

        private VisualElement GetLabelInHierarchy()
        {
            try
            {
                return this.Q(null, Constants.AddItemObjectField)
                    .hierarchy[0]
                    .hierarchy[0]
                    .hierarchy[1];
            }
            catch (NullReferenceException)
            {
            }

            return null;
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
            if (IsInitialised || serializedProperty is null)
            {
                return;
            }

            ListItemType = GetItemType(serializedProperty);

            eventHandler.Handler = new PropertyModificationHandler(serializedProperty, Reset);

            UseObjectFieldIfTypeIsUnityObject();

            AddArraySizeField();

            LoadItemTemplate();

            RegisterCallbacks();

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

        private void UseObjectFieldIfTypeIsUnityObject()
        {
            VisualElement addItemSection = this.Q(null, Constants.AddItemSection);
            if (addItemSection is null)
            {
                return;
            }

            ObjectField objectField = addItemSection.Q<ObjectField>(null, Constants.AddItemObjectField);
            Button addButton = addItemSection.Q<Button>(null, Constants.AddButtonClassName);

            if (DoNotUseObjectField || !ListItemType.IsSubclassOf(typeof(Object)))
            {
                addButton.style.display = DisplayStyle.Flex;
                objectField.style.display = DisplayStyle.None;
            }
            else
            {
                addButton.style.display = DisplayStyle.None;
                objectField.style.display = DisplayStyle.Flex;
                objectField.objectType = ListItemType;
            }
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

        [SuppressMessage("ReSharper", "HeapView.DelegateAllocation")]
        private void RegisterCallbacks()
        {
            try
            {
                RegisterButtonDelegatedCallback<AddItemEvent>(
                    GetButton(Constants.AddButtonClassName),
                    eventHandler.OnAddItem);
                RegisterButtonDelegatedCallback<ClearListRequestedEvent>(
                    GetButton(Constants.DeleteAllButtonClassName),
                    eventHandler.OnClearListRequested);
                RegisterButtonDelegatedCallback<ClearListEvent>(
                    GetButton(Constants.DeleteConfirmButtonClassName),
                    eventHandler.OnClearList);
                RegisterButtonDelegatedCallback<ClearListCancelledEvent>(
                    GetButton(Constants.DeleteCancelButtonClassName),
                    eventHandler.OnClearListCancelled);
            }
            catch (MissingFieldException e)
            {
                if (TemplateName == Constants.DefaultTemplateName)
                {
                    Debug.LogWarningFormat("Default template is missing field: {0}", e.Message);
                }
            }

            RegisterCallback<MoveItemEvent>(eventHandler.OnMoveItem);
            RegisterCallback<RemoveItemEvent>(eventHandler.OnRemoveItem);

            RegisterCallback<ClickEvent>(OnItemButtonClicked);

            RegisterAddObjectFieldCallback();
        }

        private Button GetButton(string className)
        {
            if (!(this.Q(null, className) is Button button))
            {
                throw new MissingFieldException(nameof(Button), className);
            }

            return button;
        }

        private void RegisterButtonDelegatedCallback<T2>(Button button, EventCallback<T2> endAction,
            Func<T2> eventCreator = null)
            where T2 : EventBase<T2>, new()
        {
            if (button is null)
            {
                throw new ArgumentNullException(nameof(button));
            }

            void DoSendEvent(ClickEvent evt)
            {
                if (eventCreator is null)
                {
                    SendEvent(new T2 {target = this});
                }
                else
                {
                    T2 e = eventCreator();
                    e.target = this;
                    SendEvent(e);
                }
            }

            button.RegisterCallback<ClickEvent>(DoSendEvent);
            RegisterCallback(endAction);
        }

        private void RegisterAddObjectFieldCallback()
        {
            void SendObjectAddEvent(ChangeEvent<Object> e)
            {
                SendEvent(new AddItemEvent {target = this, Item = e.newValue});
            }

            this.Q<ObjectField>(null, Constants.AddItemObjectField)?
                .RegisterCallback<ChangeEvent<Object>>(SendObjectAddEvent);
        }

        private void OnItemButtonClicked(ClickEvent evt)
        {
            ListRowElement element;
            if (!(evt.target is Button button) || (element = button.GetFirstAncestorOfType<ListRowElement>()) is null)
            {
                return;
            }

            if (button.ClassListContains(Constants.DeleteItemButtonClassName))
            {
                SendEvent(new RemoveItemEvent
                {
                    target = this, Index = element.Index
                });
            }
            else if (button.ClassListContains(Constants.MoveUpButtonClassName))
            {
                SendEvent(new MoveItemEvent
                {
                    target = this,
                    Index = element.Index,
                    Direction = MoveItemEvent.MoveDirection.Up
                });
            }
            else if (button.ClassListContains(Constants.MoveDownButtonClassName))
            {
                SendEvent(new MoveItemEvent
                {
                    target = this,
                    Index = element.Index,
                    Direction = MoveItemEvent.MoveDirection.Down
                });
            }
        }

        #endregion

        #region Reset

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
            DisableClearListButtonIfRequired();

            PopulateList();

            OnReset?.Invoke();
        }

        private void DisableClearListButtonIfRequired()
        {
            Button button = this.Q<Button>(null, Constants.DeleteAllButtonClassName);
            if (serializedProperty.arraySize == 0)
            {
                button?.SetEnabled(false);
            }
            else if (!(button is null || button.enabledSelf))
            {
                button.SetEnabled(true);
            }
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
                VisualElement itemRow = CreateItemRow(i);

                DisableReorderButtonIfRequired(itemRow, i, serializedProperty.arraySize);

                listContainer.Add(itemRow);
            }
        }

        private VisualElement CreateItemRow(int index)
        {
            ListRowElement itemRow = new ListRowElement(index);
            itemTemplate.CloneTree(itemRow);

            itemRow.Q<PropertyField>().BindProperty(serializedProperty.GetArrayElementAtIndex(index));

            return itemRow;
        }

        private static void DisableReorderButtonIfRequired(VisualElement itemRow, int index, int arraySize)
        {
            if (index == 0)
            {
                itemRow.Q<Button>(null, Constants.MoveUpButtonClassName)?.SetEnabled(false);
            }

            if (index == arraySize - 1 || arraySize <= 1)
            {
                itemRow.Q<Button>(null, Constants.MoveDownButtonClassName)?.SetEnabled(false);
            }
        }

        public void RemoveItem(int index)
        {
            SendEvent(new RemoveItemEvent
            {
                target = this,
                Index = index
            });
        }

        public void MoveItemUp(int index)
        {
            SendEvent(new MoveItemEvent
            {
                target = this,
                Direction = MoveItemEvent.MoveDirection.Up,
                Index = index
            });
        }

        public void MoveItemDown(int index)
        {
            SendEvent(new MoveItemEvent
            {
                target = this,
                Direction = MoveItemEvent.MoveDirection.Down,
                Index = index
            });
        }

        public void AddNewItemToList()
        {
            SendEvent(new AddItemEvent {target = this});
        }

        public void ClearListItems()
        {
            SendEvent(new ClearListEvent {target = this});
        }

        #endregion

        #region Uxml

        // ReSharper disable once UnusedMember.Global
        public new class UxmlFactory : UxmlFactory<ListElement, UxmlTraits>
        {}

        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            private readonly UxmlBoolAttributeDescription doNotUseObjectField;
            private readonly UxmlBoolAttributeDescription hidePropertyLabel;
            private readonly UxmlStringAttributeDescription itemTemplateName;
            private readonly UxmlStringAttributeDescription label;
            private readonly UxmlStringAttributeDescription styleSheetName;
            private readonly UxmlStringAttributeDescription templateName;

            public UxmlTraits()
            {
                label = new UxmlStringAttributeDescription {name = "label"};
                itemTemplateName = new UxmlStringAttributeDescription {name = "item-template-name"};
                styleSheetName = new UxmlStringAttributeDescription {name = "stylesheet-name"};
                templateName = new UxmlStringAttributeDescription {name = "template-name"};
                hidePropertyLabel = new UxmlBoolAttributeDescription {name = "hide-property-label"};
                doNotUseObjectField = new UxmlBoolAttributeDescription {name = "do-not-use-object-field"};
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
                le.DoNotUseObjectField = doNotUseObjectField.GetValueFromBag(bag, cc);

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

        #endregion
    }
}