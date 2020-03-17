using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Internal
{
    public class Controls : IOuterControls
    {
        private readonly VisualElement root;
        private readonly ListElementOptions options;

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

        public Label AddObjectFieldLabel => GetLabelOfObjectField(AddObjectField);

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

        private static Label GetLabelOfObjectField(VisualElement field)
        {
            if (field is null ||
                field.childCount == 0 ||
                field.hierarchy.childCount == 0 ||
                field.hierarchy[0].hierarchy.childCount < 2
            )
            {
                return null;
            }

            return field.hierarchy[0].hierarchy[0].hierarchy[1] as Label;
        }

        public Controls(VisualElement rootElement, ListElementOptions options)
        {
            root = rootElement;
            this.options = options;
            Row = new RowElements(root, options);
        }

        public class RowElements
        {
            private readonly ListElementOptions options;
            private readonly VisualElement root;

            public RowElements(VisualElement rootElement, ListElementOptions options)
            {
                root = rootElement;
                this.options = options;
            }

            public RowButtonsElementsSet this[int index] => new RowButtonsElementsSet(root, options, index);

            public class RowButtonsElementsSet : IRowButtons
            {
                private readonly VisualElement root;
                private readonly ListElementOptions options;

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
                        if (PropertyField is null || PropertyField.hierarchy.childCount == 0 ||
                            PropertyField.hierarchy[0].hierarchy.childCount == 0)
                        {
                            return null;
                        }

                        if (!(PropertyField.hierarchy[0] is ObjectField objectField))
                        {
                            return PropertyField.hierarchy[0].hierarchy[0] as Label;
                        }

                        return objectField.hierarchy[0] as Label; //.hierarchy[1] as Label;
                    }
                }

                public RowButtonsElementsSet(VisualElement listElement, ListElementOptions options, int index)
                {
                    this.options = options;
                    root = listElement.Q(null, options.ItemsSectionClassName)[index];
                }
            }
        }
    }
}