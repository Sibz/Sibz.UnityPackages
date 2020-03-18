namespace Sibz.ListElement.Events
{
    public class MoveItemEvent : ItemEventBase<MoveItemEvent>
    {
        public enum MoveDirection
        {
            Up,
            Down
        }

        public MoveDirection Direction;
    }
}