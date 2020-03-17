using System.Collections;
using NUnit.Framework;
using Sibz.ListElement.Events;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Integration.ListElementEventHandler
{
    public class RegisterCallbacks
    {

        [UnityTest]
        public IEnumerator ShouldRegisterAllCallbacks()
        {
            VisualElement element = new VisualElement();

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
            element.SendEvent(new ChangeEvent<Object>() {target = element});

            yield return null;
            
            WindowFixture.Window.rootVisualElement.Remove(element);
                
            Assert.IsEmpty(handler.EventNames);
        }
    }
}