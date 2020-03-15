using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sibz.ListElement
{
    public class Controls
    {
        private readonly VisualElement root;
        private readonly ListElementOptionsInternal options;

        public readonly RowElements Row;

        private readonly Dictionary<string, VisualElement> cache = new Dictionary<string, VisualElement>();

        public Label HeaderLabel =>
            GetElementByClassNameCached<Label>(root, options.HeaderLabelClassName);

        public Button Add =>
            GetElementByClassNameCached<Button>(root, options.AddItemButtonClassName);

        public Button ClearList =>
            GetElementByClassNameCached<Button>(root, options.ClearListButtonClassName);

        public Button ClearListConfirm =>
            GetElementByClassNameCached<Button>(root, options.ClearListConfirmButtonClassName);

        public Button ClearListCancel =>
            GetElementByClassNameCached<Button>(root, options.ClearListCancelButtonClassName);

        public ObjectField AddObjectField =>
            GetElementByClassNameCached<ObjectField>(root, options.AddItemObjectFieldClassName);

        public Label AddObjectFieldLabel
        {
            get
            {
                if (AddObjectField is null ||
                    AddObjectField.childCount == 0 ||
                    AddObjectField.hierarchy.childCount == 0 ||
                    AddObjectField.hierarchy[0].hierarchy.childCount < 2
                )
                {
                    return null;
                }

                return AddObjectField.hierarchy[0].hierarchy[0].hierarchy[1] as Label;
            }
        }

        public VisualElement HeaderSection =>
            GetElementByClassNameCached<VisualElement>(root, options.HeaderSectionClassName);

        public VisualElement ClearListConfirmSection =>
            GetElementByClassNameCached<VisualElement>(root, options.ClearListConfirmSectionClassName);

        public VisualElement ItemsSection =>
            GetElementByClassNameCached<VisualElement>(root, options.ItemsSectionClassName);

        public VisualElement AddSection =>
            GetElementByClassNameCached<VisualElement>(root, options.AddItemSectionClassName);

        private T GetElementByClassNameCached<T>(VisualElement rootElement, string className)
            where T : VisualElement
        {
            if (cache.ContainsKey(className))
            {
                return cache[className] as T;
            }

            if (!(rootElement.Q(null, className) is T element))
            {
                return null;
            }

            cache.Add(className, element);
            return element;
        }

        public Controls(VisualElement rootElement, ListElementOptionsInternal options)
        {
            root = rootElement;
            this.options = options;
            Row = new RowElements(root, options);
        }

        public class RowElements
        {
            private readonly ListElementOptionsInternal options;
            private readonly VisualElement root;

            public RowElements(VisualElement rootElement, ListElementOptionsInternal options)
            {
                root = rootElement;
                this.options = options;
            }

            public RowElementsSet this[int index] => new RowElementsSet(root, options, index);

            public class RowElementsSet
            {
                private readonly VisualElement root;
                private readonly ListElementOptionsInternal options;
                private readonly VisualElement listElement;

                public Button MoveUp =>
                    root.Q<Button>(null, options.MoveItemUpButtonClassName);

                public Button MoveDown =>
                    root.Q<Button>(null, options.MoveItemDownButtonClassName);

                public Button RemoveItem =>
                    root.Q<Button>(null, options.RemoveItemButtonClassName);

                public PropertyField PropertyField =>
                    root.Q<PropertyField>();

                public Label PropertyFieldLabel
                {
                    get
                    {
                        if (PropertyField is null || PropertyField.childCount == 0 ||
                            PropertyField.hierarchy.childCount == 0)
                        {
                            return null;
                        }

                        return PropertyField.hierarchy[0].hierarchy[0] as Label;
                    }
                }

                public RowElementsSet(VisualElement listElement, ListElementOptionsInternal options, int index)
                {
                    this.listElement = listElement;
                    this.options = options;
                    root = listElement.Q(null, options.ItemsSectionClassName)[index];
                }
            }
        }
    }
}