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

        public readonly Controls Controls;

        public readonly ListElementOptionsInternal Options;
        public bool IsInitialised { get; private set; }
        public Type ListItemType { get; private set; }
        public event Action OnReset;

        #region Attribute Properties

        private string Label => Options.Label;
        private string TemplateName => Options.TemplateName;
        private string ItemTemplateName => Options.ItemTemplateName;
        private string StyleSheetName => Options.StyleSheetName;
        private bool HidePropertyLabel => Options.HidePropertyLabel;
        private bool DoNotUseObjectField => Options.DoNotUseObjectField;
        private bool EnableReordering => Options.EnableReordering;
        private bool EnableDeletions => Options.EnableDeletions;

        #endregion

        #region Construction

        public ListElement() : this(null)
        {
        }

        public ListElement(SerializedProperty property) : this(
            string.Empty, property)
        {
        }

        public ListElement(string label, SerializedProperty property, IListElementEventHandler evtHandler = null) :
            this(property,
                new ListElementOptions {Label = label}, evtHandler)
        {
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        public ListElement(SerializedProperty property, ListElementOptions options,
            IListElementEventHandler evtHandler = null)
        {
            Controls = new Controls(this);

            serializedProperty = property;

            Options = options ?? new ListElementOptionsInternal();

            eventHandler = evtHandler ?? new ListElementEventHandler(this);

            InitialiseAndBindIfPropertyIsDefined();
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
            if (Controls.HeaderLabel is null)
            {
                return;
            }

            if (string.IsNullOrEmpty(Options.Label) && !(serializedProperty is null))
            {
                Controls.HeaderLabel.text = ObjectNames.NicifyVariableName(serializedProperty.name);
            }
            else if (string.IsNullOrEmpty(Options.Label) && serializedProperty is null)
            {
                Controls.HeaderLabel.text = "<List Name>";
            }
            else
            {
                Controls.HeaderLabel.text = Label;
            }
        }

        private void SetObjectFieldLabelText()
        {
            Label label = Controls.AddObjectFieldLabel;
            if (label is null)
            {
                return;
            }

            label.parent.style.justifyContent = Justify.Center;
            label.style.display = DisplayStyle.None;
            label.parent.Add(new Label("Drop here to add new item") {pickingMode = PickingMode.Ignore});
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
            
            SetLabelText();

            SetObjectFieldLabelText();

            ApplyOptions();

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
            if (Controls.Add is null || Controls.AddObjectField == null)
            {
                return;
            }

            if (DoNotUseObjectField || !ListItemType.IsSubclassOf(typeof(Object)))
            {
                Controls.Add.style.display = DisplayStyle.Flex;
                Controls.AddObjectField.style.display = DisplayStyle.None;
            }
            else
            {
                Controls.Add.style.display = DisplayStyle.None;
                Controls.AddObjectField.style.display = DisplayStyle.Flex;
                Controls.AddObjectField.objectType = ListItemType;
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
                    Controls.Add,
                    eventHandler.OnAddItem);
                RegisterButtonDelegatedCallback<ClearListRequestedEvent>(
                    Controls.ClearList,
                    eventHandler.OnClearListRequested);
                RegisterButtonDelegatedCallback<ClearListEvent>(
                    Controls.ClearListConfirm,
                    eventHandler.OnClearList);
                RegisterButtonDelegatedCallback<ClearListCancelledEvent>(
                    Controls.ClearListCancel,
                    eventHandler.OnClearListCancelled);
            }
            catch (MissingFieldException e)
            {
                if (TemplateName == ListElementOptionsInternal.DefaultTemplateName)
                {
                    Debug.LogWarningFormat("Default template is missing field: {0}", e.Message);
                }
            }

            RegisterCallback<MoveItemEvent>(eventHandler.OnMoveItem);
            RegisterCallback<RemoveItemEvent>(eventHandler.OnRemoveItem);

            RegisterCallback<ClickEvent>(OnItemButtonClicked);

            RegisterAddObjectFieldCallback();
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

            Controls.AddObjectField?.RegisterCallback<ChangeEvent<Object>>(SendObjectAddEvent);
        }

        private void OnItemButtonClicked(ClickEvent evt)
        {
            ListRowElement element;
            if (!(evt.target is Button button) || (element = button.GetFirstAncestorOfType<ListRowElement>()) is null)
            {
                return;
            }

            if (button.ClassListContains(Options.RemoveItemButtonClassName))
            {
                SendEvent(new RemoveItemEvent
                {
                    target = this, Index = element.Index
                });
            }
            else if (button.ClassListContains(Options.MoveItemUpButtonClassName))
            {
                SendEvent(new MoveItemEvent
                {
                    target = this,
                    Index = element.Index,
                    Direction = MoveItemEvent.MoveDirection.Up
                });
            }
            else if (button.ClassListContains(Options.MoveItemDownButtonClassName))
            {
                SendEvent(new MoveItemEvent
                {
                    target = this,
                    Index = element.Index,
                    Direction = MoveItemEvent.MoveDirection.Down
                });
            }
        }

        private void ApplyOptions()
        {
            if (!EnableDeletions)
            {
                Controls.HeaderSection.AddToClassList("hide-remove-buttons");
                Controls.ItemsSection.AddToClassList("hide-remove-buttons");
            }

            if (!EnableReordering)
            {
                Controls.ItemsSection.AddToClassList("hide-reorder-buttons");
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
            if (serializedProperty.arraySize == 0)
            {
                Controls.ClearList?.SetEnabled(false);
            }
            else if (!Controls.ClearList.enabledSelf)
            {
                Controls.ClearList.SetEnabled(true);
            }
        }

        private void PopulateList()
        {
            if (!serializedProperty.isArray)
            {
                Debug.LogWarning("Bound property is not an array type");
                return;
            }

            Controls.ItemsSection.Clear();

            for (int i = 0; i < serializedProperty.arraySize; i++)
            {
                VisualElement itemRow = CreateItemRow(i);

                Controls.ItemsSection.Add(itemRow);

                DisableReorderButtonIfRequired(i, serializedProperty.arraySize);

                HidePropertyLabelIfRequired(i);
            }
        }

        private void HidePropertyLabelIfRequired(int i)
        {
            if (HidePropertyLabel)
            {
                Controls.Row[i].PropertyField.AddToClassList(ListElementOptionsInternal.HidePropertyLabelClassName);
            }
        }

        private VisualElement CreateItemRow(int index)
        {
            ListRowElement itemRow = new ListRowElement(index);
            itemTemplate.CloneTree(itemRow);

            itemRow.Q<PropertyField>().BindProperty(serializedProperty.GetArrayElementAtIndex(index));

            return itemRow;
        }

        private void DisableReorderButtonIfRequired(int index, int arraySize)
        {
            if (index == 0)
            {
                Controls.Row[index].MoveUp?.SetEnabled(false);
            }

            if (index == arraySize - 1 || arraySize <= 1)
            {
                Controls.Row[index].MoveDown?.SetEnabled(false);
            }
        }

        #endregion

        #region Public Methods

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
        {
        }

        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            private readonly UxmlBoolAttributeDescription doNotUseObjectField;
            private readonly UxmlBoolAttributeDescription hidePropertyLabel;
            private readonly UxmlBoolAttributeDescription enableReordering;
            private readonly UxmlBoolAttributeDescription enableDeletions;
            private readonly UxmlStringAttributeDescription itemTemplateName;
            private readonly UxmlStringAttributeDescription label;
            private readonly UxmlStringAttributeDescription styleSheetName;
            private readonly UxmlStringAttributeDescription templateName;

            public UxmlTraits()
            {
                ListElementOptionsInternal defaults = new ListElementOptionsInternal();
                label = new UxmlStringAttributeDescription {name = "label", defaultValue = defaults.Label};
                itemTemplateName = new UxmlStringAttributeDescription
                    {name = "item-template-name", defaultValue = defaults.ItemTemplateName};
                styleSheetName = new UxmlStringAttributeDescription
                    {name = "stylesheet-name", defaultValue = defaults.StyleSheetName};
                templateName = new UxmlStringAttributeDescription
                    {name = "template-name", defaultValue = defaults.TemplateName};
                hidePropertyLabel = new UxmlBoolAttributeDescription
                    {name = "hide-property-label", defaultValue = defaults.HidePropertyLabel};
                doNotUseObjectField = new UxmlBoolAttributeDescription
                    {name = "do-not-use-object-field", defaultValue = defaults.DoNotUseObjectField};
                enableReordering = new UxmlBoolAttributeDescription
                    {name = "enable-reordering", defaultValue = defaults.EnableReordering};
                enableDeletions = new UxmlBoolAttributeDescription
                    {name = "enable-deletions", defaultValue = defaults.EnableDeletions};
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                if (!(ve is ListElement le))
                {
                    return;
                }

                le.Options.Label = label.GetValueFromBag(bag, cc);
                le.Options.HidePropertyLabel = hidePropertyLabel.GetValueFromBag(bag, cc);
                le.Options.DoNotUseObjectField = doNotUseObjectField.GetValueFromBag(bag, cc);
                le.Options.EnableDeletions = enableDeletions.GetValueFromBag(bag, cc);
                le.Options.EnableReordering = enableReordering.GetValueFromBag(bag, cc);

                string itn = itemTemplateName.GetValueFromBag(bag, cc);
                string ssn = styleSheetName.GetValueFromBag(bag, cc);
                string tn = templateName.GetValueFromBag(bag, cc);

                if (!string.IsNullOrEmpty(itn))
                {
                    le.Options.ItemTemplateName = itn;
                }

                if (!string.IsNullOrEmpty(ssn))
                {
                    le.Options.StyleSheetName = ssn;
                }

                if (!string.IsNullOrEmpty(tn))
                {
                    le.Options.TemplateName = tn;
                }

                le.Initialise();
            }
        }

        #endregion
    }
}