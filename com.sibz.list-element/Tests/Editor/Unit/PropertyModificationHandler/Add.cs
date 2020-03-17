using NUnit.Framework;
using Sibz.ListElement.Tests.Unit;
using UnityEditor;
using UnityEngine;
using Handler = Sibz.ListElement.Internal.PropertyModificationHandler;
// ReSharper disable HeapView.BoxingAllocation

namespace Sibz.ListElement.Tests.Unit.PropertyModificationHandler
{
    public class Add : PropertyModificationHandlerBase
    {
        [Test]
        public void ShouldAddItemToList()
        {
            Property.ClearArray();
            Handler.Add();
            Assert.AreEqual(1, Property.arraySize);
        }

        [Test]
        public void ShouldAddObjectItemToList()
        {
            ObjectProperty.ClearArray();
            ObjectHandler.Add(new Object());
            Assert.AreEqual(1, ObjectProperty.arraySize);
        }
    }
}