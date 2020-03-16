using System.Collections;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using UnityEditor.UIElements;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests
{
    [SuppressMessage("ReSharper", "ConvertToNullCoalescingCompoundAssignment")]
    [SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
    public class OptionApplicatorTests : ListElementTestBase
    {
        private new ListElementOptions options;
        private ListElement listElement;
        private Controls Controls => ListElement.Controls;

        private new ListElement ListElement
        {
            get
            {
                if (listElement is null)
                {
                    listElement = new ListElement(Property, options);
                    TestWindow.rootVisualElement.Add(listElement);
                }

                return listElement;
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
            if (listElement is null)
            {
                return;
            }

            TestWindow.rootVisualElement.Remove(listElement);
            listElement = null;
        }

        [UnityTest]
        public IEnumerator ShouldHideClearListWhenSpecified()
        {
            options.EnableDeletions = false;
            OptionApplicator.ApplyEnableDeletions(ListElement);
            yield return null;
            Assert.AreEqual(
                DisplayStyle.None,
                Controls.ClearList.resolvedStyle.display);
        }

        [UnityTest]
        public IEnumerator ShouldShowClearListNormally()
        {
            OptionApplicator.ApplyEnableDeletions(ListElement);
            yield return null;
            Assert.AreEqual(
                DisplayStyle.Flex,
                Controls.ClearList.resolvedStyle.display);
        }

        [UnityTest]
        public IEnumerator ShouldHideRemoveButtonsWhenSpecified([Values(0, 1, 2)] int row)
        {
            options.EnableDeletions = false;
            OptionApplicator.ApplyEnableDeletions(ListElement);
            yield return null;
            Assert.AreEqual(
                DisplayStyle.None,
                Controls.Row[row].RemoveItem.resolvedStyle.display);
        }

        [UnityTest]
        public IEnumerator ShouldShowRemoveButtonsNormally([Values(0, 1, 2)] int row)
        {
            OptionApplicator.ApplyEnableDeletions(ListElement);
            yield return null;
            Assert.AreEqual(
                DisplayStyle.Flex,
                Controls.Row[row].RemoveItem.resolvedStyle.display);
        }

        [UnityTest]
        public IEnumerator ShouldHideMoveButtonsWhenSpecified([Values(0, 1, 2)] int row)
        {
            options.EnableReordering = false;
            OptionApplicator.ApplyEnableReordering(ListElement);
            yield return null;
            Assert.AreEqual(
                DisplayStyle.None,
                Controls.Row[row].MoveDown.resolvedStyle.display);
            Assert.AreEqual(
                DisplayStyle.None,
                Controls.Row[row].MoveUp.resolvedStyle.display);
        }

        [UnityTest]
        public IEnumerator ShouldShowMoveButtonsNormally([Values(0, 1, 2)] int row)
        {
            OptionApplicator.ApplyEnableReordering(ListElement);
            yield return null;
            Assert.AreEqual(
                DisplayStyle.Flex,
                Controls.Row[row].MoveDown.resolvedStyle.display);
            Assert.AreEqual(
                DisplayStyle.Flex,
                Controls.Row[row].MoveUp.resolvedStyle.display);
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

        [UnityTest]
        public IEnumerator ShouldSetObjectFieldTypeToTypeOfListItem()
        {
            ListElement testElement = new ListElement(ObjectProperty, options);
            OptionApplicator.ApplyTypeToObjectField(testElement);
            yield return null;
            Assert.AreEqual(
                typeof(CustomObject),
                testElement.Controls.AddObjectField.objectType);
        }

        [Test]
        public void ShouldDisableButtonWhenCountIs0()
        {
            Button button = new Button();
            OptionApplicator.DisableButtonWhenCountIsNonZero(button, 0);
            Assert.IsFalse(button.enabledSelf);
            
            
        }
        
        [Test]
        public void ShouldEnableButtonWhenCountIsNotZero()
        {
            Button button = new Button();
            OptionApplicator.DisableButtonWhenCountIsNonZero(button, 1);
            Assert.IsTrue(button.enabledSelf);
        }

        [Test]
        public void ShouldNotErrorTryingToSetTypeOnNullObjectField()
        {
            OptionApplicator.SetTypeOnObjectField(null, typeof(CustomObject));
        }
        
        [Test]
        public void ShouldSetTypeOnObjectField()
        {
            ObjectField field = new ObjectField();
            OptionApplicator.SetTypeOnObjectField(field, typeof(CustomObject));
            Assert.AreSame(
                typeof(CustomObject),
                field.objectType);
        }
    }
}