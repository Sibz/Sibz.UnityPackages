using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Sibz.ListElement.Tests
{
    public class TestHelpers
    {
        public class TestObject : Object
        {
        }

        public class TestComponent : MonoBehaviour
        {
            public List<string> myList = new List<string> {"item1", "item2", "item3"};

            public List<TestObject> myCustomList = new List<TestObject>
                {new TestObject(), new TestObject(), new TestObject()};
        }

        public static SerializedProperty GetProperty(string name = nameof(TestComponent.myList))
        {
            return new SerializedObject(new GameObject().AddComponent<TestComponent>()).FindProperty(name);
        }
    }
}