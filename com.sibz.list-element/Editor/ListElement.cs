using System;
using Sibz.ListElement.Events;
using Sibz.ListElement.Internal;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sibz.ListElement
{
    public class ListElement : BindableElement
    {
        private IListElementEventHandler eventHandler;
        private IRowGenerator rowGenerator;
        private Type listItemType;

        public Controls Controls;

        internal SerializedProperty SerializedProperty { get; private set; }
        public readonly Internal.ListElementOptions Options = new ListElementOptions();
        public bool IsInitialised { get; private set; }

        public virtual IRowGenerator RowGenerator
        {
            get => rowGenerator;
            set => rowGenerator = value;
        }

        public Type ListItemType
        {
            get
            {
                if (listItemType != null)
                {
                    return listItemType;
                }

                var propertyPath = SerializedProperty.propertyPath.Split('.');
                object baseObject = SerializedProperty.serializedObject.targetObject;

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

                Type baseType = baseObject.GetType();
                return listItemType = baseType.IsGenericType
                    ? baseType.GetGenericArguments()[0]
                    : baseType;
            }
        }

        public string ListName => SerializedProperty == null ? "" : SerializedProperty.displayName;

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

        public ListElement() : this(false)
        {
        }

        public ListElement(bool empty)
        {
            if (!empty)
            {
                SingleAssetLoader.Load<VisualTreeAsset>(TemplateName)
                    .CloneTree(this);
            }
        }

        // ReSharper disable once UnusedMember.Global
        public ListElement(SerializedProperty property, string label) :
            this(property,
                new ListElementOptions {Label = label})
        {
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        public ListElement(SerializedProperty property, ListElementOptions options = null)
        {
            SerializedProperty = property ?? throw new ArgumentNullException(nameof(property));

            Options = options ?? Options;

            this.BindProperty(SerializedProperty);
        }

        #endregion

        #region Initialisation

        private void Initialise()
        {
            if (IsInitialised || SerializedProperty is null)
            {
                return;
            }

            Clear();

            SingleAssetLoader.Load<VisualTreeAsset>(TemplateName)
                .CloneTree(this);

            Controls = new Controls(this, Options);
            eventHandler = eventHandler ?? new ListElementEventHandler(Controls);
            eventHandler.Handler = new PropertyModificationHandler(SerializedProperty);
            rowGenerator = new RowGenerator(Options.ItemTemplateName);

            ElementInteractions.InsertHiddenIntFieldWithPropertyPathSet(this,
                SerializedProperty.FindPropertyRelative("Array.size").propertyPath);
            ListElementEventHandler.RegisterCallbacks(this, eventHandler);
            ElementInteractions.ApplyOptions(this);

            IsInitialised = true;
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

            SerializedProperty = property;

            if (IsInitialised)
            {
                SendEvent(new ListResetEvent {target = this});
            }
            else
            {
                Initialise();
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
                Internal.ListElementOptions defaults = new Internal.ListElementOptions();
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
            }
        }

        #endregion
    }
}