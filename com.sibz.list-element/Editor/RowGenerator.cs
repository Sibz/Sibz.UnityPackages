using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sibz.ListElement
{
    public class RowGenerator : IRowGenerator
    {
        private readonly VisualTreeAsset template;

        public RowGenerator(string itemTemplateName)
        {
            template = SingleAssetLoader.SingleAssetLoader.Load<VisualTreeAsset>(itemTemplateName);
        }

        public ListRowElement NewRow(int index, SerializedProperty property)
        {
            ListRowElement row = new ListRowElement(index);
            template.CloneTree(row);
            row.Q<PropertyField>()?.BindProperty(property.GetArrayElementAtIndex(index));
            return row;
        }

        public void PostInsert(Controls.RowElements.RowElementsSet rowElementsSet, int index, int arraySize)
        {
            AdjustReorderButtonsState(rowElementsSet?.MoveUp, rowElementsSet?.MoveDown, index, arraySize);
        }

        public static void AdjustReorderButtonsState(Button moveUp, Button moveDown, int index, int arraySize)
        {
            moveUp?.SetEnabled(index != 0);
            moveDown?.SetEnabled(index < arraySize - 1);
        }
    }
}