using System.Collections.Generic;
using NUnit.Framework;
using Sibz.ListElement.Events;
using UnityEditor;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Acceptance
{
    public class AddItem
    {
        private ListElement listElement;
        private static SerializedProperty Property => TestHelpers.GetProperty();

        private static readonly IEnumerable<ListOptions> WorkingOptionSet =
            AcceptanceFixture.GetWorkingOptionSetExcl(nameof(ListOptions.EnableDeletions));

        private Button Add => listElement.Controls.Add;

        [Test]
        public void ShouldAddItemToList([ValueSource(nameof(WorkingOptionSet))] ListOptions options, [Values(TestHelpers.CmdType.Click, TestHelpers.CmdType.Program)] TestHelpers.CmdType cmdType)
        {
            SerializedProperty property = Property;
            listElement = new ListElement(property, options);
            WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                int initialSize = property.arraySize;
                Add.SendEvent(new ClickEvent{target = Add});
                listElement.SendEvent(new ListResetEvent {target = listElement});
                Assert.AreEqual(initialSize+1, listElement.Controls.ItemsSection.childCount);
            });
        }

        // TODO AddItem(object)
        [Test]
        public void ShouldAddObjectToList([ValueSource(nameof(WorkingOptionSet))] ListOptions options)
        {

        }

        // TODO Enable Additions
        [Test]
        public void WhenDisabled_ShouldNotWork([ValueSource(nameof(WorkingOptionSet))] ListOptions options)
        {

        }

        // TODO Enable Additions
        [Test]
        public void ShouldDisplayButtonAndFieldBasedOnOption([ValueSource(nameof(WorkingOptionSet))] ListOptions options, [Values(true,false)] bool option)
        {

        }
    }
}