﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Handler = Sibz.ListElement.Internal.PropertyModificationHandler;
using Object = UnityEngine.Object;

namespace Sibz.ListElement.Tests.Unit.PropertyModificationHandler
{
    [SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
    public class PropertyModificationHandlerTests
    {
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
            property = TestHelpers.GetProperty();
            handler = new Handler(property);
        }
    }
}