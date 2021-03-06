﻿using NUnit.Framework;
using UnityEditor;

namespace Sibz.ListElement.Tests.Unit.ListElementTests
{
    public class Count
    {
        [Test]
        public void ShouldEqualPropertyCount()
        {
            SerializedProperty property = TestHelpers.GetProperty();
            Assert.AreEqual(property.arraySize, new ListElement(property).Count);
        }
    }
}