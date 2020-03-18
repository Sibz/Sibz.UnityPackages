using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Internal
{
    public static class ElementInteractions
    {
        public static void ApplyOptions(ListElement le)
        {
            Controls ctl = le.Controls;
            ListElementOptions opts = le.Options;
            
            SetPropertyLabelVisibility(ctl.ItemsSection, opts.HidePropertyLabel);
            SetRemoveButtonVisibility(le, opts.EnableDeletions);
            SetReorderButtonVisibility(ctl.ItemsSection, opts.EnableReordering);
            SetAddFieldVisibility(ctl.AddSection, le.ListItemType, opts.DoNotUseObjectField);
            InsertLabelInObjectField(ctl.AddObjectField, "Drop to add new item");
            SetHeaderLabelText(ctl.HeaderLabel, le.ListName, opts.Label);
            LoadAndAddStyleSheet(le, opts.StyleSheetName, opts.TemplateName);
            SetTypeOnObjectField(ctl.AddObjectField, le.ListItemType);
            
            void OnReset() => SetButtonStateBasedOnZeroIndex(le.Controls.ClearList, le.Controls.ItemsSection.childCount);
            le.OnReset += OnReset;
        }
        
        public static void SetPropertyLabelVisibility(VisualElement itemSection, bool hidePropertyLabelOption)
        {
            if (!hidePropertyLabelOption)
            {
                itemSection.RemoveFromClassList(UxmlClassNames.HidePropertyLabel);
            }
        }
        
        public static void SetRemoveButtonVisibility(VisualElement listElement, bool enableDeletionsOption)
        {
            if (!enableDeletionsOption)
            {
                listElement?.AddToClassList(UxmlClassNames.HideRemoveButtons);
            }
        }

        public static void SetReorderButtonVisibility(VisualElement itemSection, bool enableReorderingOption)
        {
            if (!enableReorderingOption)
            {
                itemSection?.AddToClassList(UxmlClassNames.HideReorderButtons);
            }
        }

        public static void SetAddFieldVisibility(
            VisualElement itemSection, System.Type type, bool doNotUseObjectFieldOption)
        {
            if (doNotUseObjectFieldOption)
            {
                return;
            }

            if (type.IsSubclassOf(typeof(Object)))
            {
                itemSection?.AddToClassList(UxmlClassNames.UseObjectField);
            }
        }

        public static void InsertLabelInObjectField(ObjectField objectField, string text)
        {
            if (objectField is null || objectField.childCount == 0 || objectField.hierarchy[0].childCount == 0)
            {
                return;
            }
            objectField.hierarchy[0].hierarchy[0].Add(new Label(text) {pickingMode = PickingMode.Ignore});
        }

        public static void SetHeaderLabelText(Label label, string listName, string optionsLabel)
        {
            if (label is null)
            {
                return;
            }
            label.text = string.IsNullOrEmpty(optionsLabel) ? listName : optionsLabel; 
        }

        public static void LoadAndAddStyleSheet(VisualElement element, string stylesheetName, string templateName)
        {
            if (stylesheetName == templateName)
            {
                return;
            }

            try
            {
                element.styleSheets.Add(
                    SingleAssetLoader.Load<StyleSheet>(stylesheetName));
            }
            catch (System.Exception e)
            {
                Debug.LogWarningFormat("Could not load custom style sheet:\n{0}", e.Message);
            }
        }

        public static void SetTypeOnObjectField(ObjectField field, System.Type type)
        {
            if (field is null)
            {
                return;
            }
            
            field.objectType = type;
        }

        
        public static void SetAddObjectFieldValueToNull(ObjectField field)
        {
            field?.SetValueWithoutNotify(null);
        }

        public static void SetConfirmSectionVisibility(Button clearListButton, VisualElement clearListSection, bool show)
        {
            if (clearListSection == null ||
                clearListButton == null)
            {
                return;
            }

            clearListSection.style.display =
                show ? DisplayStyle.Flex : DisplayStyle.None;
            clearListButton.style.display =
                show ? DisplayStyle.None : DisplayStyle.Flex;
        }
        
        public static void SetButtonStateBasedOnZeroIndex(Button button, int index)
        {
            button?.SetEnabled(index != 0);
        }
        
        public static void SetButtonStateBasedOnBeingLastPositionInArray(Button button, int index, int arraySize)
        {
            button?.SetEnabled(index < arraySize - 1);
        }
    }
}