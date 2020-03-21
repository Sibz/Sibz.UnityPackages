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
    public class ClearList
    {
        private ListElement listElement;
        private static SerializedProperty Property => TestHelpers.GetProperty();

        private static readonly IEnumerable<ListOptions> WorkingOptionSet =
            AcceptanceFixture.GetWorkingOptionSetExcl(nameof(ListOptions.EnableDeletions));

        private Button ClearButton => listElement.Controls.ClearList;
        private Button Yes => listElement.Controls.ClearListConfirm;
        private Button No => listElement.Controls.ClearListCancel;
        private VisualElement Section => listElement.Controls.ClearListConfirmSection;

        [Test]
        public void ShouldShowConfirmSection([ValueSource(nameof(WorkingOptionSet))]
            ListOptions options)
        {
            listElement = new ListElement(Property, options);
            WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                ClearButton.SendEvent(new ClickEvent {target = ClearButton});
                Assert.AreEqual(DisplayStyle.Flex, Section.resolvedStyle.display);
            });
        }

        [Test]
        public void WhenConfirmed_ShouldClearTheList([ValueSource(nameof(WorkingOptionSet))]
            ListOptions options)
        {
            SerializedProperty property = Property;
            listElement = new ListElement(property, options);

            WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                ClearButton.SendEvent(new ClickEvent {target = ClearButton});
                Yes.SendEvent(new ClickEvent {target = Yes});
                listElement.SendEvent(new ListResetEvent {target = listElement});
                Assert.AreEqual(0, property.arraySize); // TODO Replace with listElement.ListItemCount
                Assert.AreEqual(0, listElement.Controls.ItemsSection.childCount);
            });
        }

        [Test]
        public void WhenConfirmed_ShouldHideConfirmSection([ValueSource(nameof(WorkingOptionSet))]
            ListOptions options)
        {
            listElement = new ListElement(Property, options);
            WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                ClearButton.SendEvent(new ClickEvent {target = ClearButton});
                Yes.SendEvent(new ClickEvent {target = Yes});
                Assert.AreEqual(DisplayStyle.None, Section.resolvedStyle.display);
            });
        }

        [Test]
        public void WhenCancelled_ShouldNotClearList([ValueSource(nameof(WorkingOptionSet))]
            ListOptions options)
        {
            SerializedProperty property = Property;
            listElement = new ListElement(property, options);
            WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                int initialSize = property.arraySize;
                ClearButton.SendEvent(new ClickEvent {target = ClearButton});
                No.SendEvent(new ClickEvent {target = No});
                listElement.SendEvent(new ListResetEvent {target = listElement});
                Assert.AreEqual(initialSize, property.arraySize); // TODO Replace with listElement.ListItemCount
                Assert.AreEqual(initialSize, listElement.Controls.ItemsSection.childCount);
            });
        }

        [Test]
        public void WhenCancelled_ShouldHideConfirmSection([ValueSource(nameof(WorkingOptionSet))]
            ListOptions options)
        {
            listElement = new ListElement(Property, options);
            WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                ClearButton.SendEvent(new ClickEvent {target = ClearButton});
                No.SendEvent(new ClickEvent {target = No});
                Assert.AreEqual(DisplayStyle.None, Section.resolvedStyle.display);
            });
        }

        [Test]
        public void WhenListEmpty_ShouldDisableButton([ValueSource(nameof(WorkingOptionSet))]
            ListOptions options)
        {
            listElement = new ListElement(Property, options);
            WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                if (!ClearButton.enabledSelf)
                {
                    Assert.Fail("ClearList button should initially be enabled with items on list");
                }

                listElement.ClearListItems();
                listElement.SendEvent(new ListResetEvent {target = listElement});
                Assert.IsFalse(ClearButton.enabledSelf);
            });
        }

        [Test]
        public void ShouldNotClearListWhenDisabled()
        {
            SerializedProperty property = Property;
            listElement = new ListElement(Property, new ListOptions {EnableDeletions = false});
            WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                int initialSize = property.arraySize;
                listElement.ClearListItems();
                listElement.SendEvent(new ListResetEvent {target = listElement});
                Assert.AreEqual(initialSize, property.arraySize);
            });
        }

        [UnityTest]
        public IEnumerator ShouldCorrectButtonVisibilityBasedOnOption([ValueSource(nameof(WorkingOptionSet))]
            ListOptions options, [Values(true, false)] bool option)
        {
            options.EnableDeletions = option;
            listElement = new ListElement(Property, options);
            WindowFixture.RootElement.Add(listElement);
            return WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                if (ClearButton.resolvedStyle.display == (option ? DisplayStyle.None : DisplayStyle.Flex))
                {
                    Assert.Fail($"Clear List button {(option ? "should" : "should not")} be visible");
                }

                if (listElement.Query(null, UxmlClassNames.RemoveItemButtonClassName).ToList().Any(x =>
                    x.resolvedStyle.display == (option ? DisplayStyle.None : DisplayStyle.Flex)))
                {
                    Assert.Fail($"Remove item buttons {(option ? "should" : "should not")} be visible");
                }

                return null;
            });
        }
    }
}