﻿using System;
using Sibz.ListElement.Events;
using Sibz.ListElement.Internal;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Sibz.ListElement
{
    public class ListElement : BindableElement
    {
        private IListElementEventHandler eventHandler;
        private IRowGenerator rowGenerator;
        private Type listItemType;

        public Controls Controls;

        internal SerializedProperty SerializedProperty { get; private set; }
        public readonly ReadOnlyOptions Options = new ReadOnlyOptions(new ListOptions());
        public bool IsInitialised { get; private set; }
        public int Count => SerializedProperty.arraySize;

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
        private bool EnableRowLabel => Options.EnableRowLabel;
        private bool EnableObjectField => Options.EnableObjectField;
        private bool EnableReordering => Options.EnableReordering;
        private bool EnableDeletions => Options.EnableDeletions;
        private bool EnableAdditions => Options.EnableAdditions;

        private bool EnableModify => Options.EnableModify;
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
                new ListOptions {Label = label})
        {
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        public ListElement(SerializedProperty property, ListOptions options = null)
        {
            SerializedProperty = property ?? throw new ArgumentNullException(nameof(property));

            Options = options is null ? Options : new ReadOnlyOptions(options);

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

            SingleAssetLoader.Load<VisualTreeAsset>(Options.TemplateName)
                .CloneTree(this);

            Controls = new Controls(this);
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
            if (EnableDeletions)
            {
                SendEvent(new RemoveItemEvent
                {
                    target = this,
                    Index = index
                });
            }
        }

        public void MoveItemUp(int index)
        {
            MoveItem(index, MoveItemEvent.MoveDirection.Up);
        }

        public void MoveItemDown(int index)
        {
            MoveItem(index, MoveItemEvent.MoveDirection.Down);
        }

        private void MoveItem(int index, MoveItemEvent.MoveDirection direction)
        {
            if (EnableReordering)
            {
                SendEvent(new MoveItemEvent
                {
                    target = this,
                    Direction = direction,
                    Index = index
                });
            }
        }

        public void AddNewItemToList(Object obj = null)
        {
            if (EnableAdditions)
            {
                if (obj != null && obj.GetType() != ListItemType)
                {
                    throw new ArgumentException($"Expected type: {ListItemType.Name}", nameof(obj));
                }

                SendEvent(new AddItemEvent {target = this, Item = obj});
            }
        }

        public void ClearListItems()
        {
            if (EnableDeletions)
            {
                SendEvent(new ClearListEvent {target = this});
            }
        }

        public SerializedProperty GetPropertyAt(int index)
        {
            if (index < 0 || index >= SerializedProperty.arraySize)
            {
                throw new IndexOutOfRangeException();
            }

            return SerializedProperty.GetArrayElementAtIndex(index);
        }

        #endregion

        #region Uxml

        // ReSharper disable once UnusedMember.Global
        public new class UxmlFactory : UxmlFactory<ListElement, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            private readonly UxmlBoolAttributeDescription enableObjectField;
            private readonly UxmlBoolAttributeDescription enableRowLabel;
            private readonly UxmlBoolAttributeDescription enableReordering;
            private readonly UxmlBoolAttributeDescription enableDeletions;
            private readonly UxmlBoolAttributeDescription enableAdditions;
            private readonly UxmlBoolAttributeDescription enableModify;
            private readonly UxmlStringAttributeDescription itemTemplateName;
            private readonly UxmlStringAttributeDescription label;
            private readonly UxmlStringAttributeDescription styleSheetName;
            private readonly UxmlStringAttributeDescription templateName;

            public UxmlTraits()
            {
                ListOptions defaults = new ListOptions();
                label = new UxmlStringAttributeDescription {name = "label", defaultValue = defaults.Label};
                itemTemplateName = new UxmlStringAttributeDescription
                    {name = "item-template-name", defaultValue = defaults.ItemTemplateName};
                styleSheetName = new UxmlStringAttributeDescription
                    {name = "stylesheet-name", defaultValue = defaults.StyleSheetName};
                templateName = new UxmlStringAttributeDescription
                    {name = "template-name", defaultValue = defaults.TemplateName};
                enableRowLabel = new UxmlBoolAttributeDescription
                    {name = "enable-row-label", defaultValue = defaults.EnableRowLabel};
                enableObjectField = new UxmlBoolAttributeDescription
                    {name = "enable-object-field", defaultValue = defaults.EnableObjectField};
                enableReordering = new UxmlBoolAttributeDescription
                    {name = "enable-reordering", defaultValue = defaults.EnableReordering};
                enableDeletions = new UxmlBoolAttributeDescription
                    {name = "enable-deletions", defaultValue = defaults.EnableDeletions};
                enableAdditions = new UxmlBoolAttributeDescription
                    {name = "enable-additions", defaultValue = defaults.EnableAdditions};
                enableModify = new UxmlBoolAttributeDescription
                    {name = "enable-modify", defaultValue = defaults.EnableModify};
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                if (!(ve is ListElement le))
                {
                    return;
                }

                le.Options.BaseOptions.Label = label.GetValueFromBag(bag, cc);
                le.Options.BaseOptions.EnableRowLabel = enableRowLabel.GetValueFromBag(bag, cc);
                le.Options.BaseOptions.EnableObjectField = enableObjectField.GetValueFromBag(bag, cc);
                le.Options.BaseOptions.EnableDeletions = enableDeletions.GetValueFromBag(bag, cc);
                le.Options.BaseOptions.EnableReordering = enableReordering.GetValueFromBag(bag, cc);
                le.Options.BaseOptions.EnableAdditions = enableAdditions.GetValueFromBag(bag, cc);
                le.Options.BaseOptions.EnableModify = enableModify.GetValueFromBag(bag, cc);

                string itn = itemTemplateName.GetValueFromBag(bag, cc);
                string ssn = styleSheetName.GetValueFromBag(bag, cc);
                string tn = templateName.GetValueFromBag(bag, cc);

                if (!string.IsNullOrEmpty(itn))
                {
                    le.Options.BaseOptions.ItemTemplateName = itn;
                }

                if (!string.IsNullOrEmpty(ssn))
                {
                    le.Options.BaseOptions.StyleSheetName = ssn;
                }

                if (!string.IsNullOrEmpty(tn))
                {
                    le.Options.BaseOptions.TemplateName = tn;
                }
            }
        }

        #endregion
    }
}