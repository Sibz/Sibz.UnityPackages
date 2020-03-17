using System;
using NUnit.Framework;
using Sibz.ListElement.Tests.Unit.PropertyModificationHandler;

namespace Sibz.ListElement.Tests.PropertyModificationHandler
{
    public class MoveUp: PropertyModificationHandlerBase
    {
        [Test]
        public void ShouldMoveItemUp()
        {
            Handler.MoveUp(1);
            Assert.AreEqual("item2", Property.GetArrayElementAtIndex(0).stringValue);
        }

        [Test]
        public void WhenMovingFirstItem_ShouldFailSilentlyAndNotMoveItem()
        {
            Handler.MoveUp(0);
            Assert.AreEqual("item1", Property.GetArrayElementAtIndex(0).stringValue);
        }

        [Test]
        public void WhenIndexIsOutOfRange_ShouldThrowIndexOutOfRangeException([Values(-1,10)] int index)
        {
            try
            {
                Handler.MoveUp(index);
            }
            catch (IndexOutOfRangeException)
            {
                Assert.Pass();
            }
            Assert.Fail($"{typeof(IndexOutOfRangeException)} not thrown");
        } 
    }
}