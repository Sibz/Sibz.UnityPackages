using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sibz.ListElement.Events;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Sibz.ListElement.Tests.EventHandling
{
    public class ListElementEventHandlingTests
    {
        private TestEventHandler eventHandler;

        private ListElement listElement;
        private SerializedProperty property;
        private GameObject testGameObject;
        private SerializedObject testSerializedGameObject;
        private TestWindow testWindow;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            testWindow = EditorWindow.GetWindow<TestWindow>();
            testGameObject = Object.Instantiate(new GameObject());
        }

        [SetUp]
        public void TestSetup()
        {
            testGameObject.AddComponent<TestBehaviour>();
            testSerializedGameObject = new SerializedObject(testGameObject.GetComponent<TestBehaviour>());
            property = testSerializedGameObject.FindProperty(nameof(TestBehaviour.myList));
            eventHandler = new TestEventHandler();
            listElement = new ListElement(property, eventHandler);
            testWindow.rootVisualElement.Add(listElement);
        }

        [TearDown]
        public void TearDown()
        {
            testWindow.rootVisualElement.Clear();
            Object.DestroyImmediate(testGameObject.GetComponent<TestBehaviour>());
            testSerializedGameObject = null;
            property = null;
            listElement = null;
            eventHandler = null;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            testWindow.Close();
            Object.DestroyImmediate(testWindow);
            Object.DestroyImmediate(testGameObject);
        }

        [Test]
        public void ClearListRequested()
        {
            listElement.SendEvent(new ClearListRequestedEvent {target = listElement});
            Assert.IsTrue(eventHandler.OnClearListRequestedCalled);
        }

        [Test]
        public void ClearListRequestedViaButton()
        {
            listElement.SendEvent(new ClickEvent
                {target = listElement.Q<Button>(null, Constants.DeleteAllButtonClassName)});
            Assert.IsTrue(eventHandler.OnClearListRequestedCalled);
        }

        [Test]
        public void ClearList()
        {
            listElement.SendEvent(new ClearListEvent {target = listElement});
            Assert.IsTrue(eventHandler.OnClearListCalled);
        }

        [Test]
        public void ClearListViaButton()
        {
            listElement.SendEvent(new ClickEvent
                {target = listElement.Q<Button>(null, Constants.DeleteConfirmButtonClassName)});
            Assert.IsTrue(eventHandler.OnClearListCalled);
        }

        [Test]
        public void ClearListViaElement()
        {
            listElement.ClearListItems();
            Assert.IsTrue(eventHandler.OnClearListCalled);
        }

        [Test]
        public void ClearListCancelled()
        {
            listElement.SendEvent(new ClearListCancelledEvent {target = listElement});
            Assert.IsTrue(eventHandler.OnClearListCancelledCalled);
        }

        [Test]
        public void ClearListCancelledViaButton()
        {
            listElement.SendEvent(new ClickEvent
                {target = listElement.Q<Button>(null, Constants.DeleteCancelButtonClassName)});
            Assert.IsTrue(eventHandler.OnClearListCancelledCalled);
        }

        [Test]
        public void AddNewItem()
        {
            listElement.SendEvent(new AddItemEvent {target = listElement});
            Assert.IsTrue(eventHandler.OnAddItemCalled);
        }

        [Test]
        public void AddNewItemViaButton()
        {
            listElement.SendEvent(new ClickEvent {target = listElement.Q<Button>(null, Constants.AddButtonClassName)});
            Assert.IsTrue(eventHandler.OnAddItemCalled);
        }

        [Test]
        public void AddNewItemViaElement()
        {
            listElement.AddNewItemToList();
            Assert.IsTrue(eventHandler.OnAddItemCalled);
        }


        [Test]
        public void RemoveItem()
        {
            listElement.SendEvent(new RemoveItemEvent {target = listElement});
            Assert.IsTrue(eventHandler.OnRemoveItemCalled);
        }

        [Test]
        public void RemoveItemViaButton()
        {
            listElement
                .SendEvent(new ClickEvent {target = listElement.Q<Button>(null, Constants.DeleteItemButtonClassName)});
            Assert.IsTrue(eventHandler.OnRemoveItemCalled);
        }

        [Test]
        public void RemoveItemViaElement()
        {
            listElement.RemoveItem(0);
            Assert.IsTrue(eventHandler.OnRemoveItemCalled);
        }

        [Test]
        public void MoveItemRequested()
        {
            listElement.SendEvent(new MoveItemEvent {target = listElement});
            Assert.IsTrue(eventHandler.OnMoveItemRequestedCalled);
        }

        [Test]
        public void MoveItemViaButtonUp()
        {
            listElement.SendEvent(
                new ClickEvent {target = listElement.Q<Button>(null, Constants.MoveUpButtonClassName)});
            Assert.IsTrue(eventHandler.OnMoveItemRequestedCalled);
        }

        [Test]
        public void MoveItemViaButtonDown()
        {
            listElement.SendEvent(new ClickEvent
                {target = listElement.Q<Button>(null, Constants.MoveDownButtonClassName)});
            Assert.IsTrue(eventHandler.OnMoveItemRequestedCalled);
        }

        [Test]
        public void MoveItemViaElementUp()
        {
            listElement.MoveItemUp(0);
            Assert.IsTrue(eventHandler.OnMoveItemRequestedCalled);
        }

        [Test]
        public void MoveItemViaElementDown()
        {
            listElement.MoveItemDown(0);
            Assert.IsTrue(eventHandler.OnMoveItemRequestedCalled);
        }

        public class TestWindow : EditorWindow
        {
        }

        [Serializable]
        public class TestBehaviour : MonoBehaviour
        {
            public List<string> myList = new List<string> {"item1", "item2", "item3"};
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