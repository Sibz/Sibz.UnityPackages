using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sibz.UXMLList
{
    /// <summary>
    /// Creates and stores the elements used around the ListVisualElement
    /// </summary>
    public abstract class ListElementsFactoryBase
    {
        public static readonly string Uss_LabelClassName = "sibz-list-label";
        private Label m_Label;
        public Label Label => m_Label ?? (m_Label = CreateAndAddClassName(CreateLabelElement, Uss_LabelClassName));
        protected virtual Label CreateLabelElement()
        {
            var label = new Label();
            label.style.flexGrow = 1;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            return label;
        }

        public static readonly string Uss_AddButtonClassName = "sibz-list-add-button";
        private Button m_AddButton;
        public Button AddButton => m_AddButton ?? (m_AddButton = CreateAndAddClassName(CreateAddButton, Uss_AddButtonClassName));
        protected virtual Button CreateAddButton() => new Button();

        public static readonly string Uss_DeleteButtonClassName = "sibz-list-delete-all-button";
        private Button m_DeleteAllButton;
        public Button DeleteAllButton => m_DeleteAllButton ?? (m_DeleteAllButton = CreateAndAddClassName(CreateDeleteAllButton, Uss_DeleteButtonClassName));
        protected virtual Button CreateDeleteAllButton() => new Button();

        public static readonly string Uss_DeleteAllLabelClassName = "sibz-list-delete-all-label";
        private Label m_DeleteAllLabel;
        public Label DeleteAllLabel => m_DeleteAllLabel ?? (m_DeleteAllLabel = CreateAndAddClassName(CreateDelteAllLabelElement, Uss_DeleteAllLabelClassName));
        protected virtual Label CreateDelteAllLabelElement() => new Label("Are you sure?");

        public static readonly string Uss_DeleteAllYesButtonClassName = "sibz-list-delete-all-yes-button";
        private Button m_DeleteAllYesButton;
        public Button DeleteAllYesButton => m_DeleteAllYesButton ?? (m_DeleteAllYesButton = CreateAndAddClassName(CreateDeleteAllYesButton, Uss_DeleteAllYesButtonClassName));
        protected virtual Button CreateDeleteAllYesButton() => new Button();

        public static readonly string Uss_DeleteButtonNoClassName = "sibz-list-delete-all-no-button";
        private Button m_DeleteAllNoButton;
        public Button DeleteAllNoButton => m_DeleteAllNoButton ?? (m_DeleteAllNoButton = CreateAndAddClassName(CreateDeleteAllNoButton, Uss_DeleteButtonNoClassName));
        protected virtual Button CreateDeleteAllNoButton() => new Button();

        public static readonly string Uss_HeaderClassName = "sibz-list-header";
        private VisualElement m_HeaderSection;
        public VisualElement HeaderSection => m_HeaderSection ?? (m_HeaderSection = CreateAndAddClassName(CreateHeaderSection, Uss_HeaderClassName));
        protected virtual VisualElement CreateHeaderSection()
        {
            VisualElement header = new VisualElement();

            header.style.flexDirection = FlexDirection.Row;

            header.Add(Label);

            header.Add(DeleteAllButton);

            header.Add(AddButton);

            return header;
        }

        public static readonly string Uss_DeleteConfirmClassName = "sibz-list-delete-confirm";
        private VisualElement m_DeleteConfirmationSection;
        public VisualElement DeleteConfirmationSection => m_DeleteConfirmationSection ?? (m_DeleteConfirmationSection = CreateAndAddClassName(CreateDeleteConfirmationSection, Uss_DeleteConfirmClassName));

        protected virtual VisualElement CreateDeleteConfirmationSection()
        {
            var v = new VisualElement();
            v.Add(DeleteAllLabel);
            DeleteAllLabel.style.unityTextAlign = TextAnchor.MiddleRight;
            DeleteAllLabel.style.flexGrow = 1;
            v.Add(DeleteAllYesButton);
            v.Add(DeleteAllNoButton);
            DeleteAllYesButton.text = "Yes";
            DeleteAllNoButton.text = "No";
            v.style.flexDirection = FlexDirection.Row;
            return v;
        }

        public abstract void Init(ListVisualElement element);

        private T CreateAndAddClassName<T>(Func<T> fn, string className) where T : VisualElement
        {
            T item = fn();
            item.AddToClassList(className);
            return item;
        }
    }
}
