using NUnit.Framework;

// ReSharper disable HeapView.BoxingAllocation

namespace Sibz.ListElement.Tests.Unit.PropertyModificationHandler
{
    public class Clear : PropertyModificationHandlerBase
    {
        [Test]
        public void ShouldClearList()
        {
            Assert.Greater(Property.arraySize, 0);
            Handler.Clear();
            Assert.AreEqual(0, Property.arraySize);
        }
    }
}