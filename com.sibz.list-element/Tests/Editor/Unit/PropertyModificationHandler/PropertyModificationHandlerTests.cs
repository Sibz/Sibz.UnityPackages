using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Handler = Sibz.ListElement.Internal.PropertyModificationHandler;

namespace Sibz.ListElement.Tests.PropertyModificationHandler
{
    [SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
    public class PropertyModificationHandlerTests
    {
        private GameObject testGameObject;
        private SerializedObject testSerializedGameObject;
        private SerializedProperty property;
        private Handler handler;


        [Serializable]
        private class TestObject : MonoBehaviour
        {
            public List<string> myList = new List<string> {"item1", "item2", "item3"};
            public List<Object> myObjectList = new List<Object>();
        }

        [SetUp]
        public void SetUp()
        {
            testGameObject = Object.Instantiate(new GameObject());
            testGameObject = new GameObject();
            testGameObject.AddComponent<TestObject>();
            testSerializedGameObject = new SerializedObject(testGameObject.GetComponent<TestObject>());
            property = testSerializedGameObject.FindProperty(nameof(TestObject.myList));
            handler = new Handler(property);
        }


    }
}