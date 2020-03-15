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
            rowElementsSet?.MoveUp?.SetEnabled(index != 0);
            rowElementsSet?.MoveDown?.SetEnabled(index < arraySize - 1);
        }
    }
}