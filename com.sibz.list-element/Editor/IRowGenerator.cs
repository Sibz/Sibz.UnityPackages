using Sibz.ListElement.Internal;
using UnityEditor;

namespace Sibz.ListElement
{
    public interface IRowGenerator
    {
        ListRowElement NewRow(int index, SerializedProperty property);
        void PostInsert(IRowButtons rowButtonsElementsSet, int index, int arraySize);
    }
}