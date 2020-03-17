using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Internal
{
    public interface IOuterControls
    {
        Button Add { get; }

        Button ClearList  { get; }

        Button ClearListConfirm  { get; }

        Button ClearListCancel  { get; }
        
        ObjectField AddObjectField { get; }
        
        VisualElement ClearListConfirmSection { get; }
    }
}