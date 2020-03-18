using System.Linq;
using NUnit.Framework;
using UnityEngine.UIElements;
using Handler = Sibz.ListElement.Events.ListElementEventHandler;

// ReSharper disable HeapView.BoxingAllocation

namespace Sibz.ListElement.Tests.Unit.ListElementEventHandlerTests
{
    public class CreateRaiserDefinitionsForRow
    {
        private class Buttons : IRowButtons
        {
            public Button MoveUp { get; } = new Button();
            public Button MoveDown { get; } = new Button();
            public Button RemoveItem { get; } = new Button();
        }

        private readonly Buttons buttons = new Buttons();

        [SetUp]
        public void SetUp()
        {
            ListElement listElement = new ListElement(true);
            listElement.Add(buttons.MoveUp);
            listElement.Add(buttons.MoveDown);
            listElement.Add(buttons.RemoveItem);
        }

        [Test]
        public void ShouldCreateOneDefForEachButton()
        {
            var defs = Handler.CreateRaiserDefinitionsForRow(buttons, 0).ToArray();
            Assert.AreEqual(3, defs.Length);
            Assert.AreEqual(1, defs.Count(x => x.Control == buttons.MoveUp));
            Assert.AreEqual(1, defs.Count(x => x.Control == buttons.MoveDown));
            Assert.AreEqual(1, defs.Count(x => x.Control == buttons.RemoveItem));
        }

        [Test]
        public void ShouldNotCreateFailingRaiseEvents()
        {
            var defs = Handler.CreateRaiserDefinitionsForRow(buttons, 0).ToArray();
            Assert.AreEqual(3, defs.Length);

            defs[0].RaiseEvent();
            defs[1].RaiseEvent();
            defs[2].RaiseEvent();
        }
    }
}