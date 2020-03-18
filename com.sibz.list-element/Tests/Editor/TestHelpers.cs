using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Sibz.ListElement.Tests
{
    public class TestHelpers
    {
        public class TestObject : Object
        {
        }

        [Serializable]
        public class TestComponent : ScriptableObject
        {
            public List<string> myList = new List<string> {"item1", "item2", "item3"};

            public List<TestObject> myCustomList = new List<TestObject>
                {new TestObject(), new TestObject(), new TestObject()};
        }

        public static SerializedProperty GetProperty(string name = nameof(TestComponent.myList))
        {
            return new SerializedObject(ScriptableObject.CreateInstance<TestComponent>()).FindProperty(name);
        }
    }

    public static class TestHelpersExtensions
    {
        public static void AddAndRemove(this VisualElement root, VisualElement element, Action beforeRemove = null)
        {
            root.Add(element);
            beforeRemove?.Invoke();
            root.Remove(element);
        }
    }
}