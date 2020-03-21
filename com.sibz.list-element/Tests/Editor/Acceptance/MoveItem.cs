using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sibz.ListElement.Events;
using Sibz.ListElement.Internal;
using UnityEditor;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Acceptance
{

    public class MoveItem
    {
        private ListElement listElement;
        private static SerializedProperty Property => TestHelpers.GetProperty();
        private static readonly IEnumerable<ListOptions> WorkingOptionSet =
            AcceptanceFixture.GetWorkingOptionSetExcl(nameof(ListOptions.EnableReordering));

        public enum CmdType
        {
            Click,
            Program
        }


        [Test]
        public void MoveItemFromTopToBottom_ShouldWork([ValueSource(nameof(WorkingOptionSet))] ListOptions options, [Values(CmdType.Click,CmdType.Program)] CmdType cmdType)
        {
            listElement = new ListElement(Property, options);
            WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                string itemBeingMoved = listElement.GetPropertyAt(0).stringValue;
                for (int i = 0; i < Property.arraySize - 1; i++)
                {
                    if (cmdType == CmdType.Click)
                    {
                        listElement.Controls.Row[i].MoveDown.SendEvent(new ClickEvent
                            {target = listElement.Controls.Row[i].MoveDown});
                    }
                    else
                    {
                        listElement.MoveItemDown(i);
                    }

                    if (itemBeingMoved == listElement.GetPropertyAt(i + 1).stringValue)
                    {
                        continue;
                    }

                    Assert.Fail($"Move down from index {i} failed");
                }
            });
        }

        [Test]
        public void MoveItemFromBottomToTop_ShouldWork([ValueSource(nameof(WorkingOptionSet))] ListOptions options, [Values(CmdType.Click,CmdType.Program)] CmdType cmdType)
        {
            listElement = new ListElement(Property, options);
            WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                string itemBeingMoved = listElement.GetPropertyAt(Property.arraySize - 1).stringValue;
                for (int i = Property.arraySize - 1; i > 0; i--)
                {
                    if (cmdType == CmdType.Click)
                    {
                        listElement.Controls.Row[i].MoveUp.SendEvent(new ClickEvent
                            {target = listElement.Controls.Row[i].MoveUp});
                    }
                    else
                    {
                        listElement.MoveItemUp(i);
                    }

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
            MoveItemEvent.MoveDirection direction, [ValueSource(nameof(WorkingOptionSet))] ListOptions options)
        {
            listElement = new ListElement(Property, options);
            string val1 = Property.GetArrayElementAtIndex(0).stringValue;
            string val2 = Property.GetArrayElementAtIndex(Property.arraySize - 1).stringValue;
            int count = Property.arraySize;

            WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                if (direction == MoveItemEvent.MoveDirection.Up)
                {
                    listElement.Controls.Row[0].MoveUp.SendEvent(new ClickEvent { target = listElement.Controls.Row[0].MoveUp });
                }
                else
                {
                    listElement.Controls.Row[Property.arraySize - 1].MoveDown.SendEvent(new ClickEvent { target = listElement.Controls.Row[Property.arraySize - 1].MoveDown });
                }

                if (count != Property.arraySize ||
                    val1 != Property.GetArrayElementAtIndex(0).stringValue ||
                    val2 != Property.GetArrayElementAtIndex(Property.arraySize - 1).stringValue)
                {
                    Assert.Fail("Order must have changed");
                }
            });
        }

        [Test]
        public void MoveUpButtonForTopRow_ShouldBeDisabled([ValueSource(nameof(WorkingOptionSet))] ListOptions options)
        {
            listElement = new ListElement(Property, options);
            WindowFixture.RootElement.AddAndRemove(listElement,
                () => { Assert.IsFalse(listElement.Controls.Row[0].MoveUp.enabledSelf); });
        }

        [Test]
        public void MoveDownButtonForLastRow_ShouldBeDisabled([ValueSource(nameof(WorkingOptionSet))] ListOptions options)
        {
            listElement = new ListElement(Property, options);
            WindowFixture.RootElement.AddAndRemove(listElement,
                () => { Assert.IsFalse(listElement.Controls.Row[Property.arraySize - 1].MoveDown.enabledSelf); });
        }

        [Test]
        public void WhenDisabled_MoveShouldNotWork()
        {
            listElement = new ListElement(Property, new ListOptions {EnableReordering = false});
            string val1 = Property.GetArrayElementAtIndex(0).stringValue;
            string val2 = Property.GetArrayElementAtIndex(Property.arraySize - 1).stringValue;
            int count = Property.arraySize;

            WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                listElement.Controls.Row[Property.arraySize - 1].MoveUp.SendEvent(new ClickEvent { target = listElement.Controls.Row[Property.arraySize - 1].MoveUp });
                listElement.Controls.Row[0].MoveDown.SendEvent(new ClickEvent { target = listElement.Controls.Row[0].MoveDown });

                if (count != Property.arraySize ||
                    val1 != Property.GetArrayElementAtIndex(0).stringValue ||
                    val2 != Property.GetArrayElementAtIndex(Property.arraySize - 1).stringValue)
                {
                    Assert.Fail("Order must have changed");
                }
            });
        }

        [UnityTest]
        public IEnumerator ShouldDisplayBasedOnOptions([Values(true, false)] bool enableReordering)
        {
            listElement = new ListElement(Property, new ListOptions {EnableReordering = enableReordering});
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