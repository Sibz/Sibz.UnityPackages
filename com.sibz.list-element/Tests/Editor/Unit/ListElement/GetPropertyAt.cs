using System;
using NUnit.Framework;
using UnityEditor;

namespace Sibz.ListElement.Tests.Unit.ListElementTests
{
    public class GetPropertyAt
    {
        private readonly SerializedProperty property = TestHelpers.GetProperty();

        [Test]
        public void ShouldGetProperty([Values(0, 1, 2)] int row)
        {
            Assert.AreEqual(property.GetArrayElementAtIndex(row).stringValue,
                new ListElement(property).GetPropertyAt(row).stringValue);
        }

        [Test]
        public void WhenIndexOutOfRange_ShouldThrowIndexOutOfRangeException([Values(-1, 1000)] int index)
        {
            try
            {
                new ListElement(property).GetPropertyAt(index);
            }
            catch (IndexOutOfRangeException)
            {
                Assert.Pass();
                return;
            }

            Assert.Fail($"{nameof(IndexOutOfRangeException)} not thrown");
        }
    }
}