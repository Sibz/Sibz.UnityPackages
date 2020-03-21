using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Sibz.ListElement.Tests
{
    public class TestHelpers
    {
        public const string DefaultTestLabel = "Test";
        public const string DefaultTestTemplateName = "sibz.list-element.tests.template";
        public const string DefaultTestItemTemplateName = "sibz.list-element.tests.item-template";
        public const string DefaultCheckElementName = "TestCheckElement";

        public const string
            DefaultTestTemplateWthOptionsSetName = "sibz.list-element.tests.list-element-test-with-options-set";

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

        public static IEnumerator AddAndRemove(this VisualElement root, VisualElement element, Func<IEnumerator> beforeRemove = null)
        {
            root.Add(element);
            yield return null;
            yield return beforeRemove?.Invoke();
            root.Remove(element);
        }
    }
}