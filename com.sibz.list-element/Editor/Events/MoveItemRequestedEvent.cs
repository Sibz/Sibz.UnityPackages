namespace Sibz.ListElement.Events
{
    public class MoveItemRequestedEvent : ItemRequestEventBase<MoveItemRequestedEvent>
    {
        public MoveDirection Direction;

        public enum MoveDirection
        {
            Up,
            Down
        }
    }
}