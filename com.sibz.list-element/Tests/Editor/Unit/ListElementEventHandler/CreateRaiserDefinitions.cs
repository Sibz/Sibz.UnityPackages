using System.Linq;
using NUnit.Framework;
using Sibz.ListElement.Internal;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Handler = Sibz.ListElement.Events.ListElementEventHandler;

// ReSharper disable HeapView.BoxingAllocation

namespace Sibz.ListElement.Tests.Unit.ListElementEventHandler
{
    public class CreateRaiserDefinitions
    {
        private class Controls : IOuterControls
        {
            public Button Add { get; } = new Button();
            public Button ClearList { get; } = new Button();
            public Button ClearListConfirm { get; } = new Button();
            public Button ClearListCancel { get; } = new Button();
            public ObjectField AddObjectField { get; } = new ObjectField();
            public VisualElement ClearListConfirmSection { get; } = new VisualElement();
        }

        private readonly Controls controls = new Controls();

        [SetUp]
        public void SetUp()
        {
            ListElement listElement = new ListElement(true);
            listElement.Add(controls.Add);
            listElement.Add(controls.ClearList);
            listElement.Add(controls.ClearListConfirm);
            listElement.Add(controls.ClearListCancel);
            listElement.Add(controls.AddObjectField);
        }

        [Test]
        public void ShouldCreateOneDefForEachButton()
        {
            var defs = Handler.CreateRaiserDefinitions(controls).ToArray();
            Assert.AreEqual(5, defs.Length);
            Assert.AreEqual(1, defs.Count(x => x.Control == controls.Add));
            Assert.AreEqual(1, defs.Count(x => x.Control == controls.ClearList));
            Assert.AreEqual(1, defs.Count(x => x.Control == controls.ClearListConfirm));
            Assert.AreEqual(1, defs.Count(x => x.Control == controls.ClearListCancel));
            Assert.AreEqual(1, defs.Count(x => x.Control == controls.AddObjectField));
        }

        [Test]
        public void ShouldNotCreateFailingRaiseEvents()
        {
            var defs = Handler.CreateRaiserDefinitions(controls).ToArray();
            Assert.AreEqual(5, defs.Length);

            defs[0].RaiseEvent();
            defs[1].RaiseEvent();
            defs[2].RaiseEvent();
            defs[3].RaiseEvent();
            defs[4].RaiseEvent();
        }
    }
}