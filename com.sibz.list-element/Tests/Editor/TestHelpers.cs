using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
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
}