using System.Collections;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests
{
    [SuppressMessage("ReSharper", "ConvertToNullCoalescingCompoundAssignment")]
    [SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
    public class OptionApplicatorTests : ListElementTestBase
    {
        private new ListElementOptions options;
        private ListElement m_ListElement;
        private Controls controls => ListElement.Controls;

        private new ListElement ListElement
        {
            get
            {
                if (m_ListElement is null)
                {
                    m_ListElement = new ListElement(Property, options);
                    TestWindow.rootVisualElement.Add(m_ListElement);
                }

                return m_ListElement;
            }
        }

        [SetUp]
        public void OptionApplicatorSetUp()
        {
            options = new ListElementOptions();
        }

        [TearDown]
        public void OptionApplicatorTearDown()
        {
            if (m_ListElement is null)
            {
                return;
            }

            TestWindow.rootVisualElement.Remove(m_ListElement);
            m_ListElement = null;
        }

        [UnityTest]
        public IEnumerator ShouldHideClearListWhenSpecified()
        {
            options.EnableDeletions = false;
            OptionApplicator.ApplyEnableDeletions(ListElement);
            yield return null;
            Assert.AreEqual(
                DisplayStyle.None,
                controls.ClearList.resolvedStyle.display);
        }

        [UnityTest]
        public IEnumerator ShouldShowClearListNormally()
        {
            OptionApplicator.ApplyEnableDeletions(ListElement);
            yield return null;
            Assert.AreEqual(
                DisplayStyle.Flex,
                controls.ClearList.resolvedStyle.display);
        }

        [UnityTest]
        public IEnumerator ShouldHideRemoveButtonsWhenSpecified([Values(0, 1, 2)] int row)
        {
            options.EnableDeletions = false;
            OptionApplicator.ApplyEnableDeletions(ListElement);
            yield return null;
            Assert.AreEqual(
                DisplayStyle.None,
                controls.Row[row].RemoveItem.resolvedStyle.display);
        }

        [UnityTest]
        public IEnumerator ShouldShowRemoveButtonsNormally([Values(0, 1, 2)] int row)
        {
            OptionApplicator.ApplyEnableDeletions(ListElement);
            yield return null;
            Assert.AreEqual(
                DisplayStyle.Flex,
                controls.Row[row].RemoveItem.resolvedStyle.display);
        }

        [UnityTest]
        public IEnumerator ShouldHideMoveButtonsWhenSpecified([Values(0, 1, 2)] int row)
        {
            options.EnableReordering = false;
            OptionApplicator.ApplyEnableReordering(ListElement);
            yield return null;
            Assert.AreEqual(
                DisplayStyle.None,
                controls.Row[row].MoveDown.resolvedStyle.display);
            Assert.AreEqual(
                DisplayStyle.None,
                controls.Row[row].MoveUp.resolvedStyle.display);
        }

        [UnityTest]
        public IEnumerator ShouldShowMoveButtonsNormally([Values(0, 1, 2)] int row)
        {
            OptionApplicator.ApplyEnableReordering(ListElement);
            yield return null;
            Assert.AreEqual(
                DisplayStyle.Flex,
                controls.Row[row].MoveDown.resolvedStyle.display);
            Assert.AreEqual(
                DisplayStyle.Flex,
                controls.Row[row].MoveUp.resolvedStyle.display);
        }

        [UnityTest]
        public IEnumerator ShouldHidePropertyLabelNormally()
        {
            OptionApplicator.ApplyHidePropertyLabel(ListElement);
            yield return null;
            Assert.AreEqual(
                DisplayStyle.None,
                ListElement.Controls.Row[0].PropertyFieldLabel.resolvedStyle.display);
        }

        [UnityTest]
        public IEnumerator ShouldShowPropertyLabelWhenSpecified()
        {
            options.HidePropertyLabel = false;
            OptionApplicator.ApplyHidePropertyLabel(ListElement);
            yield return null;
            Assert.AreEqual(
                DisplayStyle.Flex,
                ListElement.Controls.Row[0].PropertyFieldLabel.resolvedStyle.display);
        }

        [UnityTest]
        public IEnumerator ShouldHideAddObjectFieldForNoneObjectList()
        {
            OptionApplicator.ApplyDoNotUseObjectField(ListElement);
            yield return null;
            Assert.AreEqual(
                DisplayStyle.None,
                ListElement.Controls.AddObjectField.resolvedStyle.display);
        }

        [UnityTest]
        public IEnumerator ShouldHideAddObjectFieldForObjectListWhenSpecified()
        {
            options.DoNotUseObjectField = true;
            ListElement testElement = new ListElement(ObjectProperty, options);
            OptionApplicator.ApplyDoNotUseObjectField(testElement);
            yield return null;
            Assert.AreEqual(
                DisplayStyle.None,
                testElement.Controls.AddObjectField.resolvedStyle.display);
        }

        [UnityTest]
        public IEnumerator ShouldNormallyShowAddObjectFieldForObjectList()
        {
            ListElement testElement = new ListElement(ObjectProperty, options);
            OptionApplicator.ApplyDoNotUseObjectField(testElement);
            yield return null;
            Assert.AreEqual(
                DisplayStyle.Flex,
                testElement.Controls.AddObjectField.resolvedStyle.display);
        }

        [UnityTest]
        public IEnumerator ShouldSetLabelToSpecifiedLabel()
        {
            options.Label = "Test";
            OptionApplicator.ApplyLabelText(ListElement);
            yield return null;
            Assert.AreEqual(
                options.Label,
                ListElement.Controls.HeaderLabel.text);
        }

        [UnityTest]
        public IEnumerator ShouldSetLabelToPropertyNameNormally()
        {
            OptionApplicator.ApplyLabelText(ListElement);
            yield return null;
            Assert.AreEqual(
                Property.displayName,
                ListElement.Controls.HeaderLabel.text);
        }

        [UnityTest]
        public IEnumerator ShouldHideDefaultAddObjectFieldLabel()
        {
            OptionApplicator.ApplyAddObjectFieldLabel(ListElement);
            yield return null;
            Assert.AreEqual(
                DisplayStyle.None,
                ListElement.Controls.AddObjectFieldLabel.resolvedStyle.display);
        }

        [UnityTest]
        public IEnumerator ShouldHaveCustomLabelInAddObjectField()
        {
            OptionApplicator.ApplyAddObjectFieldLabel(ListElement);
            yield return null;
            Assert.AreEqual(
                4,
                ListElement.Controls.AddObjectFieldLabel.parent.childCount);
        }

        [Test]
        public void ShouldApplyStyleSheetWhenSpecifiedIsDifferentFromTemplateName()
        {
            options.StyleSheetName = "TestTemplate";
            OptionApplicator.ApplyCustomStyleSheet(ListElement);
            Assert.IsTrue(ListElement.styleSheets.Contains(
                SingleAssetLoader.SingleAssetLoader.Load<StyleSheet>(options.StyleSheetName)));
        }

        [Test]
        public void ShouldNotApplyStyleSheetWhenSpecifiedIsSameFromTemplateName()
        {
            options.TemplateName = "TestTemplate";
            options.StyleSheetName = "TestTemplate";
            OptionApplicator.ApplyCustomStyleSheet(ListElement);
            Assert.IsFalse(ListElement.styleSheets.Contains(
                SingleAssetLoader.SingleAssetLoader.Load<StyleSheet>(options.StyleSheetName)));
        }
    }
}