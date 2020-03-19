using NUnit.Framework;
using UnityEditor;

namespace Sibz.ListElement.Tests.Unit.ListElementTests.Properties
{
    public class ListName
    {
        [Test]
        public void ShouldReturnNicifiedNameOfBoundPropertyOnComponent()
        {
            Assert.AreEqual(
                ObjectNames.NicifyVariableName(nameof(TestHelpers.TestComponent.myList)),
                new ListElement(TestHelpers.GetProperty()).ListName);
        }
    }
}