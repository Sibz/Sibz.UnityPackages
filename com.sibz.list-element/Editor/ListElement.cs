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
        private IRowGenerator rowGenerator;

        public Controls Controls;

        public readonly ListElementOptionsInternal Options;
        public bool IsInitialised { get; private set; }
        public Type ListItemType { get; private set; }

        public string ListName => serializedProperty == null ? "" : serializedProperty.displayName;
        public event Action OnReset;

        #region Attribute Properties

        // ReSharper disable UnusedMember.Local
        private string Label => Options.Label;
        private string TemplateName => Options.TemplateName;
        private string ItemTemplateName => Options.ItemTemplateName;
        private string StyleSheetName => Options.StyleSheetName;
        private bool HidePropertyLabel => Options.HidePropertyLabel;
        private bool DoNotUseObjectField => Options.DoNotUseObjectField;
        private bool EnableReordering => Options.EnableReordering;

        private bool EnableDeletions => Options.EnableDeletions;
        // ReSharper restore UnusedMember.Local

        #endregion

        #region Construction

        public ListElement() : this(null)
        {
        }

        public ListElement(SerializedProperty property) : this(
            string.Empty, property)
        {
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public ListElement(string label, SerializedProperty property, IListElementEventHandler evtHandler = null) :
            this(property,
                new ListElementOptions {Label = label}, evtHandler)
        {
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        public ListElement(SerializedProperty property, ListElementOptions options,
            IListElementEventHandler evtHandler = null)
        {
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

        private void InitialiseWithSerializedProperty()
        {
            if (IsInitialised || serializedProperty is null)
            {
                return;
            }

            Controls = new Controls(this, Options);

            ListItemType = GetItemType(serializedProperty);

            eventHandler.Handler = new PropertyModificationHandler(serializedProperty, Reset);

            rowGenerator = Options.RowGenerator ?? new RowGenerator(Options.ItemTemplateName);

            OptionApplicator.ApplyAll(this);

            AddArraySizeField();

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

            if (!property.isArray)
            {
                Debug.LogWarning("Bound property is not an array type");
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

            Controls.ItemsSection.Clear();

            for (int i = 0; i < serializedProperty.arraySize; i++)
            {
                Controls.ItemsSection.Add(rowGenerator.NewRow(i, serializedProperty));
                rowGenerator.PostInsert(Controls.Row[i], i, serializedProperty.arraySize);
            }

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