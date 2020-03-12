using UnityEditor;

namespace Sibz.ListElement
{
    public class PropertyModificationHandler
    {
        private readonly SerializedProperty property;

        private readonly System.Action onModify;

        public PropertyModificationHandler(SerializedProperty property, System.Action onModify = null)
        {
            this.property = property;
            this.onModify = onModify;
        }

        private void ApplyModification()
        {
            property.serializedObject.ApplyModifiedProperties();
            onModify?.Invoke();
        }

        public void Add()
        {
            property.InsertArrayElementAtIndex(property.arraySize);
            ApplyModification();
        }

        public void Remove(int index)
        {
            if (index < 0 || index >= property.arraySize)
            {
                throw new System.IndexOutOfRangeException("Unable to delete item");
            }

            property.DeleteArrayElementAtIndex(index);
            ApplyModification();
        }

        public void MoveUp(int index)
        {
            if (index == 0)
            {
                return;
            }

            if (index < 0 || index >= property.arraySize)
            {
                throw new System.IndexOutOfRangeException("Unable to move item");
            }

            property.MoveArrayElement(index, index - 1);
            ApplyModification();
        }

        public void MoveDown(int index)
        {
            if (index == property.arraySize - 1)
            {
                return;
            }

            if (index < 0 || index >= property.arraySize)
            {
                throw new System.IndexOutOfRangeException("Unable to move item");
            }

            property.MoveArrayElement(index, index + 1);
            ApplyModification();
        }

        public void Clear()
        {
            property.ClearArray();
            ApplyModification();
        }
    }
}