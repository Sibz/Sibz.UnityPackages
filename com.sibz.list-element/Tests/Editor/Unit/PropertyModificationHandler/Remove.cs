using System;
using NUnit.Framework;

// ReSharper disable HeapView.BoxingAllocation

namespace Sibz.ListElement.Tests.Unit.PropertyModificationHandler
{
    public class Remove : PropertyModificationHandlerBase
    {
        [Test]
        public void ShouldDeleteItem()
        {
            Assert.AreEqual(3, Property.arraySize);
            Handler.Remove(0);
            Assert.AreEqual(2, Property.arraySize);
        }

        [Test]
        public void ShouldDeleteCorrectItem()
        {
            Handler.Remove(0);
            Assert.AreNotEqual("item1", Property.GetArrayElementAtIndex(0).stringValue);
        }

        [Test]
        public void WhenIndexOutOfRange_ShouldThrowIndexOutOfRangeException()
        {
            try
            {
                Handler.Remove(10);
            }
            catch (IndexOutOfRangeException)
            {
                Assert.Pass();
            }

            Assert.Fail($"{typeof(IndexOutOfRangeException)} not thrown");
        }
    }
}