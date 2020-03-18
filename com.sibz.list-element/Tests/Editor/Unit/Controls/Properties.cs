using NUnit.Framework;
using Sibz.ListElement.Internal;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Unit.Controls
{
    public class Properties : ControlsTestBase
    {
        [Test]
        public void HeaderLabel()
        {
            Assert.That(
                Controls.HeaderLabel,
                new IsElementWithClassName<Label>(UxmlClassNames.HeaderLabelClassName));
        }

        [Test]
        public void AddButton()
        {
            Assert.That(
                Controls.Add,
                new IsElementWithClassName<Button>(UxmlClassNames.AddItemButtonClassName));
        }

        [Test]
        public void ClearListButton()
        {
            Assert.That(
                Controls.ClearList,
                new IsElementWithClassName<Button>(UxmlClassNames.ClearListButtonClassName));
        }

        [Test]
        public void ClearListConfirmButton()
        {
            Assert.That(
                Controls.ClearListConfirm,
                new IsElementWithClassName<Button>(UxmlClassNames.ClearListConfirmButtonClassName));
        }

        [Test]
        public void ClearListCancelButton()
        {
            Assert.That(
                Controls.ClearListCancel,
                new IsElementWithClassName<Button>(UxmlClassNames.ClearListCancelButtonClassName));
        }

        [Test]
        public void AddObjectField()
        {
            Assert.That(
                Controls.AddObjectField,
                new IsElementWithClassName<ObjectField>(UxmlClassNames.AddItemObjectFieldClassName));
        }

        [Test]
        public void AddObjectFieldLabel()
        {
            Assert.IsNotNull(Controls.AddObjectFieldLabel);
            Assert.IsTrue(
                Controls.AddObjectFieldLabel.GetFirstAncestorOfType<ObjectField>()
                    .ClassListContains(UxmlClassNames.AddItemObjectFieldClassName));
        }

        [Test]
        public void HeaderSection()
        {
            Assert.That(
                Controls.HeaderSection,
                new IsElementWithClassName<VisualElement>(UxmlClassNames.HeaderSectionClassName));
        }

        [Test]
        public void ClearListConfirmSection()
        {
            Assert.That(
                Controls.ClearListConfirmSection,
                new IsElementWithClassName<VisualElement>(UxmlClassNames.ClearListConfirmSectionClassName));
        }

        [Test]
        public void AddSection()
        {
            Assert.That(
                Controls.AddSection,
                new IsElementWithClassName<VisualElement>(UxmlClassNames.AddItemSectionClassName));
        }

        [Test]
        public void ItemSection()
        {
            Assert.That(
                Controls.ItemsSection,
                new IsElementWithClassName<VisualElement>(UxmlClassNames.ItemsSectionClassName));
        }
    }
}