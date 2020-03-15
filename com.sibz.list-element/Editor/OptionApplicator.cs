using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sibz.ListElement
{
    public static class OptionApplicator
    {
        public static void ApplyAll(ListElement listElement)
        {
            foreach (MethodInfo methodInfo in typeof(OptionApplicator).GetMethods().Where(x =>
                x.Name != nameof(ApplyAll) && x.IsStatic && x.GetParameters().Count() == 1 &&
                x.GetParameters()[0].ParameterType == typeof(ListElement)))
            {
                methodInfo.Invoke(null, new object[] {listElement});
            }
        }

        public static void ApplyEnableDeletions(ListElement listElement)
        {
            if (!listElement.Options.EnableDeletions)
            {
                listElement.AddToClassList(listElement.Options.HideRemoveButtonsClassName);
            }
        }

        public static void ApplyHidePropertyLabel(ListElement listElement)
        {
            if (!listElement.Options.HidePropertyLabel)
            {
                listElement.Controls.ItemsSection.RemoveFromClassList(listElement.Options.HidePropertyLabelClassName);
            }
        }

        public static void ApplyEnableReordering(ListElement listElement)
        {
            if (!listElement.Options.EnableReordering)
            {
                listElement.Controls.ItemsSection.AddToClassList(listElement.Options.HideReorderButtonsClassName);
            }
        }

        public static void ApplyDoNotUseObjectField(ListElement listElement)
        {
            if (listElement.Options.DoNotUseObjectField)
            {
                return;
            }

            if (listElement.ListItemType.IsSubclassOf(typeof(Object)))
            {
                listElement.Controls.Add.style.display = DisplayStyle.None;
                listElement.Controls.AddObjectField.style.display = DisplayStyle.Flex;
            }
        }

        public static void ApplyLabelText(ListElement listElement)
        {
            listElement.Controls.HeaderLabel.text =
                string.IsNullOrEmpty(listElement.Options.Label) ? listElement.ListName : listElement.Options.Label;
        }

        public static void ApplyAddObjectFieldLabel(ListElement listElement)
        {
            Label label = listElement.Controls.AddObjectFieldLabel;
            if (label is null)
            {
                return;
            }

            label.parent.style.justifyContent = Justify.Center;
            label.style.display = DisplayStyle.None;
            label.parent.Add(new Label("Drop here to add new item") {pickingMode = PickingMode.Ignore});
        }
    }
}