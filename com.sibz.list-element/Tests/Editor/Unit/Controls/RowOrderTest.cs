using System.Linq;
using NUnit.Framework;
using UnityEngine.UIElements;
using Row = Sibz.ListElement.ListRowElement;

namespace Sibz.ListElement.Tests.Unit.Controls
{
    public class RowProperties : ControlsTestBase
    {
        
        [Test]
        public void MoveUpBuuton()
        {
            Assert.That(
                Controls.Row[0].MoveUp,
                new IsElementWithClassName<Button>(Options.MoveItemUpButtonClassName));
        }

        [Test]
        public void MoveDownButton()
        {
            Assert.That(
                Controls.Row[0].MoveDown,
                new IsElementWithClassName<Button>(Options.MoveItemDownButtonClassName));
        }

        [Test]
        public void RemoveButton()
        {
            Assert.That(
                Controls.Row[0].RemoveItem,
                new IsElementWithClassName<Button>(Options.RemoveItemButtonClassName));
        }

        [Test]
        public void PropertyField()
        {
            Assert.IsNotNull(Controls.Row[0].PropertyField);
            Assert.IsNotNull(
                Controls.Row[0].PropertyField.GetFirstAncestorOfType<Row>());
        }
        
        [Test]
        public void PropertyFieldForObjectList()
        {
            Assert.IsNotNull(ControlsForObjectList.Row[0].PropertyField);
            Assert.IsNotNull(
                ControlsForObjectList.Row[0].PropertyField.GetFirstAncestorOfType<Row>());
        }

        [Test]
        public void PropertyFieldLabel()
        {
            Assert.IsNotNull(Controls.Row[0].PropertyFieldLabel);
            Assert.IsNotNull(
                Controls.Row[0].PropertyFieldLabel.GetFirstAncestorOfType<Row>());
        }
        
        [Test]
        public void PropertyFieldLabelForObjectList()
        {
            Assert.IsNotNull(ControlsForObjectList.Row[0].PropertyField);
            Assert.IsNotNull(ControlsForObjectList.Row[0].PropertyFieldLabel);
            Assert.IsNotNull(
                ControlsForObjectList.Row[0].PropertyFieldLabel.GetFirstAncestorOfType<Row>());
        }
        
        [Test]
        public void RowFieldsBelongToCorrectRow([Values(0, 1, 2)] int row)
        {
            Assert.IsTrue(new[]
            {
                Controls.Row[row].MoveUp.GetFirstAncestorOfType<Row>().Index,
                Controls.Row[row].MoveDown.GetFirstAncestorOfType<Row>().Index,
                Controls.Row[row].RemoveItem.GetFirstAncestorOfType<Row>().Index,
                Controls.Row[row].PropertyField.GetFirstAncestorOfType<Row>().Index,
                Controls.Row[row].PropertyFieldLabel.GetFirstAncestorOfType<Row>().Index
            }.All(x => x == row));
        }
    }
}