using NUnit.Framework;

namespace Sibz.ListElement.Tests.Unit.ListElementTests.Properties
{
    public class ListItemType
    {
        [Test]
        public void WhenListItemTypeIsString_ShouldReturnStringType()
        {
            Assert.AreEqual(typeof(string), new ListElement(TestHelpers.GetProperty()).ListItemType);
        }

        [Test]
        public void WhenListItemTypeIsObject_ShouldReturnObjectType()
        {
            Assert.AreEqual(typeof(TestHelpers.TestObject), new ListElement(TestHelpers.GetProperty(nameof(TestHelpers.TestComponent.myCustomList))).ListItemType);
        }
    }
}