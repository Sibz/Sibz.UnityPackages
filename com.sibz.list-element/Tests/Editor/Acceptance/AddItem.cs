using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Sibz.ListElement.Events;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Acceptance
{
    public class AddItem
    {
        private ListElement listElement;
        private static SerializedProperty Property => TestHelpers.GetProperty();

        private static readonly IEnumerable<ListOptions> WorkingOptionSet =
            AcceptanceFixture.GetWorkingOptionSetExcl(nameof(ListOptions.EnableDeletions));

        private static readonly IEnumerable<ListOptions> ObjectFieldOptionSet =
            AcceptanceFixture.GetWorkingOptionSetExcl(new[] {nameof(ListOptions.EnableObjectField), nameof(ListOptions.TemplateName), nameof(ListOptions.ItemTemplateName), nameof(ListOptions.StyleSheetName)});

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

        [UnityTest]
        public IEnumerator ShouldDisplayFieldBasedOnOption([ValueSource(nameof(ObjectFieldOptionSet))] ListOptions options, [Values(true,false)] bool option)
        {
            options.EnableObjectField = option;
            DisplayStyle expectedDisplayStyle = option ? DisplayStyle.Flex : DisplayStyle.None;
            listElement =  new ListElement(TestHelpers.GetProperty(nameof(TestHelpers.TestComponent.myCustomList)), options);
            return WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                Debug.Log($"{options.Label}\n{options.EnableDeletions}\n{options.EnableReordering}\n{options.EnableObjectField}\n{options.HidePropertyLabel}\n{options.TemplateName}\n{options.ItemTemplateName}\n");
                Assert.AreEqual(expectedDisplayStyle, listElement.Controls.AddObjectField.resolvedStyle.display);
                return null;
            });
        }
    }
}