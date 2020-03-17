using System;
using NUnit.Framework;

namespace Sibz.ListElement.Tests.Unit.PropertyModificationHandler
{
    public class MoveDown : PropertyModificationHandlerBase
    {
        [Test]
        public void ShouldMoveItemDown()
        {
            Handler.MoveDown(1);
            Assert.AreEqual("item2", Property.GetArrayElementAtIndex(2).stringValue);
        }

        [Test]
        public void WhenMovingLastItem_ShouldFailSilentlyAndNotMoveItem()
        {
            Handler.MoveDown(2);
            Assert.AreEqual("item3", Property.GetArrayElementAtIndex(2).stringValue);
        }

        [Test]
        public void WhenIndexIsOutOfRange_ShouldThrowIndexOutOfRangeException([Values(-1,10)] int index)
        {
            try
            {
                Handler.MoveDown(index);
            }
            catch (IndexOutOfRangeException)
            {
                Assert.Pass();
            }
            Assert.Fail($"{typeof(IndexOutOfRangeException)} not thrown");
        } 
    }
}