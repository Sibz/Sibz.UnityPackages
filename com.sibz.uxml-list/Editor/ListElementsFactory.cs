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
        public static readonly string Uss_AddButtonClassName = "sibz-list-add-button";
        public static readonly string Uss_HeaderClassName = "sibz-list-header";

        private Label m_Label;
        public Label Label => m_Label ?? (m_Label = CreateAndAddClassName(CreateLabelElement, Uss_LabelClassName));

        private Button m_AddNewItemButton;
        public Button AddNewItemButton => m_AddNewItemButton ?? (m_AddNewItemButton = CreateAndAddClassName(CreateAddNewItemButton, Uss_AddButtonClassName));

        private VisualElement m_HeaderSection;
        public VisualElement HeaderSection => m_HeaderSection ?? (m_HeaderSection = CreateAndAddClassName(CreateHeaderSection, Uss_HeaderClassName));

        public abstract void Init(ListVisualElement element);

        protected virtual Label CreateLabelElement()
        {
            var label = new Label();
            label.style.flexGrow = 1;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            return label;
        }

        protected virtual Button CreateAddNewItemButton()
        {
            return new Button();
        }

        protected virtual VisualElement CreateHeaderSection()
        {
            VisualElement header = new VisualElement();

            header.style.flexDirection = FlexDirection.Row;

            header.Add(Label);

            header.Add(AddNewItemButton);

            return header;
        }

        private T CreateAndAddClassName<T>(Func<T> fn, string className) where T: VisualElement
        {
            T item = fn();
            item.AddToClassList(className);
            return item;
        }
    }
}
