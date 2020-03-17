using NUnit.Framework;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Unit.Controls
{
    public class Properties  : ControlsTestBase
    {
       
        [Test]
        public void HeaderLabel()
        {
            Assert.That(
                Controls.HeaderLabel,
                new IsElementWithClassName<Label>(Options.HeaderLabelClassName));
        }

        [Test]
        public void AddButton()
        {
            Assert.That(
                Controls.Add,
                new IsElementWithClassName<Button>(Options.AddItemButtonClassName));
        }

        [Test]
        public void ClearListButton()
        {
            Assert.That(
                Controls.ClearList,
                new IsElementWithClassName<Button>(Options.ClearListButtonClassName));
        }

        [Test]
        public void ClearListConfirmButton()
        {
            Assert.That(
                Controls.ClearListConfirm,
                new IsElementWithClassName<Button>(Options.ClearListConfirmButtonClassName));
        }

        [Test]
        public void ClearListCancelButton()
        {
            Assert.That(
                Controls.ClearListCancel,
                new IsElementWithClassName<Button>(Options.ClearListCancelButtonClassName));
        }

        [Test]
        public void AddObjectField()
        {
            Assert.That(
                Controls.AddObjectField,
                new IsElementWithClassName<ObjectField>(Options.AddItemObjectFieldClassName));
        }

        [Test]
        public void AddObjectFieldLabel()
        {
            Assert.IsNotNull(Controls.AddObjectFieldLabel);
            Assert.IsTrue(
                Controls.AddObjectFieldLabel.GetFirstAncestorOfType<ObjectField>()
                    .ClassListContains(Options.AddItemObjectFieldClassName));
        }

        [Test]
        public void HeaderSection()
        {
            Assert.That(
                Controls.HeaderSection,
                new IsElementWithClassName<VisualElement>(Options.HeaderSectionClassName));
        }

        [Test]
        public void ClearListConfirmSection()
        {
            Assert.That(
                Controls.ClearListConfirmSection,
                new IsElementWithClassName<VisualElement>(Options.ClearListConfirmSectionClassName));
        }

        [Test]
        public void AddSection()
        {
            Assert.That(
                Controls.AddSection,
                new IsElementWithClassName<VisualElement>(Options.AddItemSectionClassName));
        }

        [Test]
        public void ItemSection()
        {
            Assert.That(
                Controls.ItemsSection,
                new IsElementWithClassName<VisualElement>(Options.ItemsSectionClassName));
        }

    }
}