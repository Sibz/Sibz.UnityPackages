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
            AcceptanceFixture.GetWorkingOptionSetExcl(new[]
            {
                nameof(ListOptions.EnableObjectField), nameof(ListOptions.TemplateName),
                nameof(ListOptions.ItemTemplateName), nameof(ListOptions.StyleSheetName)
            });

        private static readonly IEnumerable<ListOptions> EnableAdditionsOptionSet =
            AcceptanceFixture.GetWorkingOptionSetExcl(new[]
            {
                nameof(ListOptions.EnableAdditions), nameof(ListOptions.TemplateName),
                nameof(ListOptions.ItemTemplateName), nameof(ListOptions.StyleSheetName)
            });

        private Button Add => listElement.Controls.Add;

        [Test]
        public void ShouldAddItemToList([ValueSource(nameof(WorkingOptionSet))]
            ListOptions options, [Values(TestHelpers.CmdType.Click, TestHelpers.CmdType.Program)]
            TestHelpers.CmdType cmdType)
        {
            SerializedProperty property = Property;
            listElement = new ListElement(property, options);
            WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                int initialSize = property.arraySize;
                Add.SendEvent(new ClickEvent {target = Add});
                listElement.SendEvent(new ListResetEvent {target = listElement});
                Assert.AreEqual(initialSize + 1, listElement.Controls.ItemsSection.childCount);
            });
        }

        [Test]
        public void ShouldAddObjectToList()
        {
            SerializedProperty property = TestHelpers.GetProperty(nameof(TestHelpers.TestComponent.myCustomList));
            listElement = new ListElement(property);
            WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                int initialSize = property.arraySize;
                TestHelpers.TestObject test = ScriptableObject.CreateInstance<TestHelpers.TestObject>();
                listElement.AddNewItemToList(test);
                Assert.AreSame(test, listElement.GetPropertyAt(initialSize).objectReferenceValue);
            });
        }

        [Test]
        public void WhenDisabled_ShouldNotWork([ValueSource(nameof(EnableAdditionsOptionSet))]
            ListOptions options)
        {
            SerializedProperty property = Property;
            listElement = new ListElement(property, options);
            WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                int initialSize = property.arraySize;
                listElement.AddNewItemToList();
                listElement.SendEvent(new ListResetEvent {target = listElement});
                Assert.AreEqual(initialSize, listElement.Controls.ItemsSection.childCount);
            });
        }

        [UnityTest]
        public IEnumerator ShouldDisplayButtonAndFieldBasedOnOption([ValueSource(nameof(EnableAdditionsOptionSet))]
            ListOptions options, [Values(true, false)] bool option)
        {
            options.EnableAdditions = option;
            DisplayStyle expectedDisplayStyle = option ? DisplayStyle.Flex : DisplayStyle.None;
            listElement = new ListElement(Property, options);
            return WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                Assert.AreEqual(expectedDisplayStyle, listElement.Controls.AddSection.resolvedStyle.display);
                return null;
            });
        }

        [UnityTest]
        public IEnumerator ShouldDisplayFieldBasedOnOption([ValueSource(nameof(ObjectFieldOptionSet))]
            ListOptions options, [Values(true, false)] bool option)
        {
            options.EnableObjectField = option;
            DisplayStyle expectedDisplayStyle = option ? DisplayStyle.Flex : DisplayStyle.None;
            listElement = new ListElement(TestHelpers.GetProperty(nameof(TestHelpers.TestComponent.myCustomList)),
                options);
            return WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                Assert.AreEqual(expectedDisplayStyle, listElement.Controls.AddObjectField.resolvedStyle.display);
                return null;
            });
        }
    }
}