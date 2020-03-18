using UnityEngine.UIElements;

namespace Sibz.ListElement
{
    public interface IRowButtons
    {
        Button MoveUp { get; }
        Button MoveDown { get; }
        Button RemoveItem { get; }
    }
}