using UnityEditor;

namespace Sibz.ListElement
{
    public interface IRowGenerator
    {
        ListRowElement NewRow(int index, SerializedProperty property);
        void PostInsert(Controls.RowElements.RowElementsSet rowElementsSet, int index, int arraySize);
    }
}