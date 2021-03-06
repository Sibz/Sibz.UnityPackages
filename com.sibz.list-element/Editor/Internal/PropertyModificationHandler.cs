﻿using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Sibz.ListElement.Internal
{
    public class PropertyModificationHandler
    {
        private readonly SerializedProperty property;

        public PropertyModificationHandler(SerializedProperty property)
        {
            this.property = property;
        }

        private void ApplyModification()
        {
            property.serializedObject.ApplyModifiedProperties();
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
                throw new IndexOutOfRangeException("Unable to delete item");
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
                throw new IndexOutOfRangeException("Unable to move item");
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
                throw new IndexOutOfRangeException("Unable to move item");
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