﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Sibz.ListElement.Internal;
using UnityEditor;
using UnityEngine;

namespace Sibz.ListElement.Tests.Integration
{
    [SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
    public class ListElementTestBase
    {
        protected static ListElementTestsFixture.TestWindow TestWindow => ListElementTestsFixture.Window;

        protected ListElement ListElement;
        protected SerializedProperty Property;
        protected SerializedProperty ObjectProperty;
        private GameObject testGameObject;
        protected SerializedObject TestSerializedGameObject;

        protected ReadOnlyOptions options => ListElement.Options;

        public class MyTestObject : MonoBehaviour
        {
            public List<string> myList = new List<string> {"item1", "item2", "item3"};

            public List<CustomObject> myCustomList = new List<CustomObject>
                {new CustomObject(), new CustomObject(), new CustomObject()};
        }

        public class CustomObject : Object
        {
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            //testGameObject = Object.Instantiate(new GameObject());
        }

        [SetUp]
        public void TestSetup()
        {
            /*testGameObject.AddComponent<MyTestObject>();
            TestSerializedGameObject =
                new SerializedObject(testGameObject.GetComponent<MyTestObject>());
            Property = TestSerializedGameObject.FindProperty(nameof(MyTestObject.myList));
            ObjectProperty = TestSerializedGameObject.FindProperty(nameof(MyTestObject.myCustomList));
            ListElement = new ListElement(Property);

            TestWindow.rootVisualElement.Add(ListElement);*/
        }

        [TearDown]
        public void TearDown()
        {
//            TestWindow.rootVisualElement.Remove(ListElement);
//            Object.DestroyImmediate(testGameObject.GetComponent<MyTestObject>());
//            TestSerializedGameObject = null;
//            Property = null;
//            ListElement = null;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
//            Object.DestroyImmediate(testGameObject);
        }
    }
}