using System;
using NUnit.Framework;
using UnityEngine;

namespace Sibz.ListElement.Tests.Unit.ListElementTests
{
    public class AddNewItem
    {
        [Test]
        public void WhenObjectIsNotListItemType_ShouldThrowArgumentException()
        {
            try
            {
                new ListElement(TestHelpers.GetProperty(nameof(TestHelpers.TestComponent.myCustomList)))
                    .AddNewItemToList(ScriptableObject.CreateInstance<TestHelpers.TestComponent>());
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("obj", e.ParamName);
                return;
            }
            Assert.Fail($"{nameof(ArgumentException)} not thrown");
        }
    }
}