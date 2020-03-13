using UnityEditor;
using UnityEngine;

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

        public void Add(Object obj = null)
        {
            property.InsertArrayElementAtIndex(property.arraySize);
            if (!(obj is null))
            {
                property
                    .GetArrayElementAtIndex(property.arraySize - 1)
                    .objectReferenceValue = obj;
            }

            ApplyModification();
        }

        public void Remove(int index)
        {
            if (index < 0 || index >= property.arraySize)
            {
                throw new System.IndexOutOfRangeException("Unable to delete item");
            }

            int initialArraySize = property.arraySize;
            property.DeleteArrayElementAtIndex(index);

            // Delete doesn't delete first time if it's an object
            if (initialArraySize == property.arraySize)
            {
                property.DeleteArrayElementAtIndex(index);
            }

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