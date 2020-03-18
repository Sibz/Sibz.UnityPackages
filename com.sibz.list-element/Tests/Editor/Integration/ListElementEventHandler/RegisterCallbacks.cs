using System.Collections;
using NUnit.Framework;
using Sibz.ListElement.Events;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Integration.ListElementEventHandlerTests
{
    public class RegisterCallbacks
    {
        [UnityTest]
        public IEnumerator ShouldRegisterAllCallbacks()
        {
            ListElement element = new ListElement(true);

            WindowFixture.Window.rootVisualElement.Add(element);

            TestEventHandler handler = new TestEventHandler();

            Events.ListElementEventHandler.RegisterCallbacks(element, handler);

            element.SendEvent(new ClearListRequestedEvent {target = element});
            element.SendEvent(new ClearListEvent {target = element});
            element.SendEvent(new ClearListCancelledEvent {target = element});
            element.SendEvent(new MoveItemEvent {target = element});
            element.SendEvent(new RemoveItemEvent {target = element});
            element.SendEvent(new AddItemEvent {target = element});
            element.SendEvent(new ClickEvent {target = element});
            element.SendEvent(new ChangeEvent<Object> {target = element});
            element.SendEvent(new ChangeEvent<int> {target = element});
            element.SendEvent(new RowInsertedEvent {target = element});
            element.SendEvent(new ListResetEvent {target = element});
            element.SendEvent(new AttachToPanelEvent {target = element});

            yield return null;

            WindowFixture.Window.rootVisualElement.Remove(element);

            Assert.IsEmpty(handler.EventNames);
        }
    }
}