using NUnit.Framework;
using Sibz.ListElement.Events;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.EventHandling
{
    public class ListElementEventHandlingTests : ListElementTestBase
    {
        private TestEventHandler eventHandler;

        [SetUp]
        public void EventTestSetup()
        {
            eventHandler = new TestEventHandler();
            ListElement = new ListElement(Property, null, eventHandler);
            TestWindow.rootVisualElement.Add(ListElement);
        }

        [TearDown]
        public void EventTearDown()
        {
            eventHandler = null;
        }

        [Test]
        public void ClearListRequested()
        {
            ListElement.SendEvent(new ClearListRequestedEvent {target = ListElement});
            Assert.IsTrue(eventHandler.OnClearListRequestedCalled);
        }

        [Test]
        public void ClearListRequestedViaButton()
        {
            ListElement.SendEvent(new ClickEvent
                {target = ListElement.Q<Button>(null, Constants.DeleteAllButtonClassName)});
            Assert.IsTrue(eventHandler.OnClearListRequestedCalled);
        }

        [Test]
        public void ClearList()
        {
            ListElement.SendEvent(new ClearListEvent {target = ListElement});
            Assert.IsTrue(eventHandler.OnClearListCalled);
        }

        [Test]
        public void ClearListViaButton()
        {
            ListElement.SendEvent(new ClickEvent
                {target = ListElement.Q<Button>(null, Constants.DeleteConfirmButtonClassName)});
            Assert.IsTrue(eventHandler.OnClearListCalled);
        }

        [Test]
        public void ClearListViaElement()
        {
            ListElement.ClearListItems();
            Assert.IsTrue(eventHandler.OnClearListCalled);
        }

        [Test]
        public void ClearListCancelled()
        {
            ListElement.SendEvent(new ClearListCancelledEvent {target = ListElement});
            Assert.IsTrue(eventHandler.OnClearListCancelledCalled);
        }

        [Test]
        public void ClearListCancelledViaButton()
        {
            ListElement.SendEvent(new ClickEvent
                {target = ListElement.Q<Button>(null, Constants.DeleteCancelButtonClassName)});
            Assert.IsTrue(eventHandler.OnClearListCancelledCalled);
        }

        [Test]
        public void AddNewItem()
        {
            ListElement.SendEvent(new AddItemEvent {target = ListElement});
            Assert.IsTrue(eventHandler.OnAddItemCalled);
        }

        [Test]
        public void AddNewItemViaButton()
        {
            ListElement.SendEvent(new ClickEvent {target = ListElement.Q<Button>(null, Constants.AddButtonClassName)});
            Assert.IsTrue(eventHandler.OnAddItemCalled);
        }

        [Test]
        public void AddNewItemViaElement()
        {
            ListElement.AddNewItemToList();
            Assert.IsTrue(eventHandler.OnAddItemCalled);
        }


        [Test]
        public void RemoveItem()
        {
            ListElement.SendEvent(new RemoveItemEvent {target = ListElement});
            Assert.IsTrue(eventHandler.OnRemoveItemCalled);
        }

        [Test]
        public void RemoveItemViaButton()
        {
            ListElement
                .SendEvent(new ClickEvent {target = ListElement.Q<Button>(null, Constants.DeleteItemButtonClassName)});
            Assert.IsTrue(eventHandler.OnRemoveItemCalled);
        }

        [Test]
        public void RemoveItemViaElement()
        {
            ListElement.RemoveItem(0);
            Assert.IsTrue(eventHandler.OnRemoveItemCalled);
        }

        [Test]
        public void MoveItemRequested()
        {
            ListElement.SendEvent(new MoveItemEvent {target = ListElement});
            Assert.IsTrue(eventHandler.OnMoveItemRequestedCalled);
        }

        [Test]
        public void MoveItemViaButtonUp()
        {
            ListElement.SendEvent(
                new ClickEvent {target = ListElement.Q<Button>(null, Constants.MoveUpButtonClassName)});
            Assert.IsTrue(eventHandler.OnMoveItemRequestedCalled);
        }

        [Test]
        public void MoveItemViaButtonDown()
        {
            ListElement.SendEvent(new ClickEvent
                {target = ListElement.Q<Button>(null, Constants.MoveDownButtonClassName)});
            Assert.IsTrue(eventHandler.OnMoveItemRequestedCalled);
        }

        [Test]
        public void MoveItemViaElementUp()
        {
            ListElement.MoveItemUp(0);
            Assert.IsTrue(eventHandler.OnMoveItemRequestedCalled);
        }

        [Test]
        public void MoveItemViaElementDown()
        {
            ListElement.MoveItemDown(0);
            Assert.IsTrue(eventHandler.OnMoveItemRequestedCalled);
        }

        private class TestEventHandler : IListElementEventHandler
        {
            public bool
                OnAddItemCalled,
                OnClearListRequestedCalled,
                OnClearListCalled,
                OnClearListCancelledCalled,
                OnRemoveItemCalled,
                OnMoveItemRequestedCalled;

            public PropertyModificationHandler Handler { get; set; }

            public void OnAddItem(AddItemEvent evt)
            {
                OnAddItemCalled = true;
            }

            public void OnClearListRequested(ClearListRequestedEvent evt)
            {
                OnClearListRequestedCalled = true;
            }

            public void OnClearList(ClearListEvent evt)
            {
                OnClearListCalled = true;
            }

            public void OnClearListCancelled(ClearListCancelledEvent evt)
            {
                OnClearListCancelledCalled = true;
            }

            public void OnRemoveItem(RemoveItemEvent evt)
            {
                OnRemoveItemCalled = true;
            }

            public void OnMoveItem(MoveItemEvent evt)
            {
                OnMoveItemRequestedCalled = true;
            }
        }
    }
}