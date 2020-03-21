using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Acceptance
{
    public class RemoveItem
    {
        private ListElement listElement;
        private static SerializedProperty Property => TestHelpers.GetProperty();

        private static readonly IEnumerable<ListOptions> WorkingOptionSet =
            AcceptanceFixture.GetWorkingOptionSetExcl(nameof(ListOptions.EnableDeletions));

        [Test]
        public void ShouldRemoveItem([ValueSource(nameof(WorkingOptionSet))]
            ListOptions options, [Values(TestHelpers.CmdType.Click, TestHelpers.CmdType.Program)]
            TestHelpers.CmdType cmdType)
        {
            listElement = new ListElement(Property, options);
            WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                int initialSize = listElement.Count;
                if (cmdType == TestHelpers.CmdType.Click)
                {
                    listElement.Controls.Row[0].RemoveItem.SendEvent(new ClickEvent
                        {target = listElement.Controls.Row[0].RemoveItem});
                }
                else
                {
                    listElement.RemoveItem(0);
                }

                Assert.AreEqual(initialSize - 1, listElement.Count);
            });
        }

        [Test]
        public void WhenDisabled_ShouldNotRemoveItem()
        {
            listElement = new ListElement(Property, new ListOptions { EnableDeletions = false});

            WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                int initialSize = listElement.Count;
                listElement.RemoveItem(0);
                Assert.AreEqual(initialSize, listElement.Count);
            });
        }
    }
}