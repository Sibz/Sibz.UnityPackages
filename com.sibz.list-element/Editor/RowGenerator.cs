using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sibz.ListElement
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class RowGenerator : IRowGenerator
    {
        private readonly VisualTreeAsset template;

        public RowGenerator(string itemTemplateName)
        {
            template = SingleAssetLoader.Load<VisualTreeAsset>(itemTemplateName);
        }

        public virtual ListRowElement NewRow(int index, SerializedProperty property)
        {
            ListRowElement row = new ListRowElement(index);
            template.CloneTree(row);
            row.Q<PropertyField>()?.BindProperty(property.GetArrayElementAtIndex(index));
            return row;
        }
    }
}