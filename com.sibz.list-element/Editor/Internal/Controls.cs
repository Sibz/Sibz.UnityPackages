using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Internal
{
    public class Controls : IOuterControls
    {
        private readonly VisualElement root;

        public readonly RowElements Row;

        private readonly Dictionary<string, VisualElement> cache = new Dictionary<string, VisualElement>();

        public Label HeaderLabel =>
            GetElementByClassNameCached<Label>(root, UxmlClassNames.HeaderLabelClassName);

        public Button Add =>
            GetElementByClassNameCached<Button>(root, UxmlClassNames.AddItemButtonClassName);

        public Button ClearList =>
            GetElementByClassNameCached<Button>(root, UxmlClassNames.ClearListButtonClassName);

        public Button ClearListConfirm =>
            GetElementByClassNameCached<Button>(root, UxmlClassNames.ClearListConfirmButtonClassName);

        public Button ClearListCancel =>
            GetElementByClassNameCached<Button>(root, UxmlClassNames.ClearListCancelButtonClassName);

        public ObjectField AddObjectField =>
            GetElementByClassNameCached<ObjectField>(root, UxmlClassNames.AddItemObjectFieldClassName);

        public Label AddObjectFieldLabel => GetLabelOfObjectField(AddObjectField);

        public VisualElement HeaderSection =>
            GetElementByClassNameCached<VisualElement>(root, UxmlClassNames.HeaderSectionClassName);

        public VisualElement ClearListConfirmSection =>
            GetElementByClassNameCached<VisualElement>(root, UxmlClassNames.ClearListConfirmSectionClassName);

        public VisualElement ItemsSection =>
            GetElementByClassNameCached<VisualElement>(root, UxmlClassNames.ItemsSectionClassName);

        public VisualElement AddSection =>
            GetElementByClassNameCached<VisualElement>(root, UxmlClassNames.AddItemSectionClassName);

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

        public Controls(VisualElement rootElement)
        {
            root = rootElement;
            Row = new RowElements(root);
        }

        public class RowElements
        {
            private readonly VisualElement root;

            public RowElements(VisualElement rootElement)
            {
                root = rootElement;
            }

            public RowButtonsElementsSet this[int index] => new RowButtonsElementsSet(root, index);

            public class RowButtonsElementsSet : IRowButtons
            {
                private readonly VisualElement root;

                public Button MoveUp =>
                    root.Q<Button>(null, UxmlClassNames.MoveItemUpButtonClassName);

                public Button MoveDown =>
                    root.Q<Button>(null, UxmlClassNames.MoveItemDownButtonClassName);

                public Button RemoveItem =>
                    root.Q<Button>(null, UxmlClassNames.RemoveItemButtonClassName);

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

                public RowButtonsElementsSet(VisualElement listElement, int index)
                {
                    root = listElement.Q(null, UxmlClassNames.ItemsSectionClassName)[index];
                }
            }
        }
    }
}