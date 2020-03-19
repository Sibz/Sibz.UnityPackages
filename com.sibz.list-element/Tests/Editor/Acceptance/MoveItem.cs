using System.Collections;
using System.Linq;
using NUnit.Framework;
using Sibz.ListElement.Events;
using Sibz.ListElement.Internal;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Acceptance
{
    public class MoveItem
    {
        private ListElement listElement;
        private readonly SerializedProperty property = TestHelpers.GetProperty();

        [SetUp]
        public void SetUp()
        {
            listElement = new ListElement(property);
        }

        [Test]
        public void MoveItemFromTopToBottom_ShouldWork()
        {
            WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                string itemBeingMoved = listElement.GetPropertyAt(0).stringValue;
                for (int i = 0; i < property.arraySize - 1; i++)
                {
                    listElement.MoveItemDown(i);
                    if (itemBeingMoved == listElement.GetPropertyAt(i + 1).stringValue)
                    {
                        continue;
                    }

                    Assert.Fail($"Move down from index {i} failed");
                }
            });
        }

        [Test]
        public void MoveItemFromBottomToTop_ShouldWork()
        {
            WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                string itemBeingMoved = listElement.GetPropertyAt(property.arraySize - 1).stringValue;
                for (int i = property.arraySize - 1; i > 0; i--)
                {
                    listElement.MoveItemUp(i);
                    if (itemBeingMoved == listElement.GetPropertyAt(i - 1).stringValue)
                    {
                        continue;
                    }

                    Assert.Fail($"Move up from index {i} failed");
                }
            });
        }

        [Test]
        public void MoveFirstUpOrLastDown_ShouldDoNothing(
            [Values(MoveItemEvent.MoveDirection.Down, MoveItemEvent.MoveDirection.Up)]
            MoveItemEvent.MoveDirection direction)
        {
            string val1 = property.GetArrayElementAtIndex(0).stringValue;
            string val2 = property.GetArrayElementAtIndex(property.arraySize - 1).stringValue;
            int count = property.arraySize;

            WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                if (direction == MoveItemEvent.MoveDirection.Up)
                {
                    listElement.MoveItemUp(0);
                }
                else
                {
                    listElement.MoveItemDown(property.arraySize - 1);
                }

                if (count != property.arraySize ||
                    val1 != property.GetArrayElementAtIndex(0).stringValue ||
                    val2 != property.GetArrayElementAtIndex(property.arraySize - 1).stringValue)
                {
                    Assert.Fail("Order must have changed");
                }
            });
        }

        [Test]
        public void MoveUpButtonForTopRow_ShouldBeDisabled()
        {
            WindowFixture.RootElement.AddAndRemove(listElement,
                () => { Assert.IsFalse(listElement.Controls.Row[0].MoveUp.enabledSelf); });
        }

        [Test]
        public void MoveDownButtonForLastRow_ShouldBeDisabled()
        {
            WindowFixture.RootElement.AddAndRemove(listElement,
                () => { Assert.IsFalse(listElement.Controls.Row[property.arraySize - 1].MoveDown.enabledSelf); });
        }

        [Test]
        public void WhenDisabled_MoveShouldNotWork()
        {
            listElement = new ListElement(property, new ListOptions {EnableReordering = false});
            string val1 = property.GetArrayElementAtIndex(0).stringValue;
            string val2 = property.GetArrayElementAtIndex(property.arraySize - 1).stringValue;
            int count = property.arraySize;

            WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                listElement.MoveItemUp(property.arraySize - 1);
                listElement.MoveItemDown(0);

                if (count != property.arraySize ||
                    val1 != property.GetArrayElementAtIndex(0).stringValue ||
                    val2 != property.GetArrayElementAtIndex(property.arraySize - 1).stringValue)
                {
                    Assert.Fail("Order must have changed");
                }
            });
        }

        [UnityTest]
        public IEnumerator ShouldDisplayBasedOnOptions([Values(true, false)] bool enableReordering)
        {
            listElement = new ListElement(property, new ListOptions {EnableReordering = enableReordering});
            WindowFixture.RootElement.Add(listElement);
            yield return null;
            WindowFixture.RootElement.Remove(listElement);
            if (listElement.Query(null, UxmlClassNames.MoveItemUpButtonClassName).ToList().Any(x =>
                    x.resolvedStyle.display == (enableReordering ? DisplayStyle.None : DisplayStyle.Flex))
                ||
                listElement.Query(null, UxmlClassNames.MoveItemDownButtonClassName).ToList().Any(x =>
                    x.resolvedStyle.display == (enableReordering ? DisplayStyle.None : DisplayStyle.Flex)))
            {
                Assert.Fail(
                    $"Buttons {(enableReordering ? "should" : "shouldn't")} be displayed with option {enableReordering}");
            }
        }
    }
}