using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sibz.ListElement
{
    public class Controls
    {
        private readonly ListElement root;
        private ListElementOptionsInternal options => root.Options;

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

        public Controls(ListElement rootElement)
        {
            root = rootElement;
            Row = new RowElements(root);
        }

        public class RowElements
        {
            private readonly ListElement root;

            public RowElements(ListElement rootElement)
            {
                root = rootElement;
            }

            public RowElementsSet this[int index] => new RowElementsSet(root, index);

            public class RowElementsSet
            {
                private readonly VisualElement root;
                private readonly ListElement listElement;

                public Button MoveUp =>
                    root.Q<Button>(null, listElement.Options.MoveItemUpButtonClassName);

                public Button MoveDown =>
                    root.Q<Button>(null, listElement.Options.MoveItemDownButtonClassName);

                public Button RemoveItem =>
                    root.Q<Button>(null, listElement.Options.RemoveItemButtonClassName);

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

                public RowElementsSet(ListElement listElement, int index)
                {
                    this.listElement = listElement;
                    root = listElement.Q(null, listElement.Options.ItemsSectionClassName)[index];
                }
            }
        }
    }
}