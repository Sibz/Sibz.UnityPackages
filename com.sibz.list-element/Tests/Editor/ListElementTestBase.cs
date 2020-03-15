using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Sibz.ListElement.Tests
{
    [SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
    public class ListElementTestBase
    {
        protected static ListElementTestsFixture.TestWindow TestWindow => ListElementTestsFixture.Window;

        protected ListElement ListElement;
        protected SerializedProperty Property;
        private GameObject testGameObject;
        protected SerializedObject TestSerializedGameObject;

        protected ListElementOptionsInternal options => ListElement.Options;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            testGameObject = Object.Instantiate(new GameObject());
        }

        [SetUp]
        public void TestSetup()
        {
            testGameObject.AddComponent<ListElementTests.MyTestObject>();
            TestSerializedGameObject =
                new SerializedObject(testGameObject.GetComponent<ListElementTests.MyTestObject>());
            Property = TestSerializedGameObject.FindProperty(nameof(ListElementTests.MyTestObject.myList));
            ListElement = new ListElement(Property);

            TestWindow.rootVisualElement.Add(ListElement);
        }

        [TearDown]
        public void TearDown()
        {
            TestWindow.rootVisualElement.Remove(ListElement);
            Object.DestroyImmediate(testGameObject.GetComponent<ListElementTests.MyTestObject>());
            TestSerializedGameObject = null;
            Property = null;
            ListElement = null;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Object.DestroyImmediate(testGameObject);
        }
    }
}